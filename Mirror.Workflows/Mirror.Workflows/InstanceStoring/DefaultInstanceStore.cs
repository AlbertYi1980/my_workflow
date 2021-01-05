using System;
using System.Activities.DurableInstancing;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using System.Xml.Linq;
using Mirror.Workflows.Activities;

// ReSharper disable UnusedParameter.Local

namespace Mirror.Workflows.InstanceStoring
{
    public class DefaultInstanceStore : InstanceStore
    {
        private readonly IInstanceRepository _repository;

        public DefaultInstanceStore(IInstanceRepository repository)
        {
            _repository = repository;
        }

        private bool KeepInstanceDataAfterCompletion => false;

        protected override IAsyncResult BeginTryCommand(
            InstancePersistenceContext context,
            InstancePersistenceCommand command,
            TimeSpan timeout, AsyncCallback callback,
            object state)
        {
            try
            {
                switch (command)
                {
                    case SaveWorkflowCommand saveWorkflowCommand:
                        return new TypedCompletedAsyncResult<bool>(SaveWorkflow(context, saveWorkflowCommand), callback,
                            state);
                    case LoadWorkflowCommand loadWorkflowCommand:
                        return new TypedCompletedAsyncResult<bool>(LoadWorkflow(context, loadWorkflowCommand), callback,
                            state);
                    case CreateWorkflowOwnerCommand createWorkflowOwnerCommand:
                        return new TypedCompletedAsyncResult<bool>(
                            CreateWorkflowOwner(context, createWorkflowOwnerCommand), callback, state);
                    case DeleteWorkflowOwnerCommand deleteWorkflowOwnerCommand:
                        return new TypedCompletedAsyncResult<bool>(
                            DeleteWorkflowOwner(context, deleteWorkflowOwnerCommand), callback, state);
                    default:
                        return new TypedCompletedAsyncResult<bool>(false, callback, state);
                }
            }
            catch (Exception e)
            {
                return new TypedCompletedAsyncResult<Exception>(e, callback, state);
            }
        }

        protected override bool EndTryCommand(IAsyncResult result)
        {
            if (result is TypedCompletedAsyncResult<Exception> exceptionResult)
            {
                throw exceptionResult.Data;
            }

            return TypedCompletedAsyncResult<bool>.End(result);
        }

        #region operations

        private bool SaveWorkflow(InstancePersistenceContext context, SaveWorkflowCommand command)
        {
            if (context.InstanceVersion == -1)
            {
                context.BindAcquiredLock(0);
            }

            if (command.CompleteInstance)
            {
                context.CompletedInstance();
                if (!KeepInstanceDataAfterCompletion)
                {
                    _repository.Delete(context.InstanceView.InstanceId);
                }
            }
            else
            {
              var foo =   command.InstanceData.Values.Skip(2).First().Value.GetType();
                
                var data = SerializeData(command.InstanceData);
                var metadata = SerializeMetadata(context.InstanceView.InstanceMetadata, command.InstanceMetadataChanges);

                _repository.Save(context.InstanceView.InstanceId, new InstanceDataPackage(metadata, data));


                foreach (var property in command.InstanceMetadataChanges)
                {
                    context.WroteInstanceMetadataValue(property.Key, property.Value);
                }

                context.PersistedInstance(command.InstanceData);
                if (command.CompleteInstance)
                {
                    context.CompletedInstance();
                }

                if (command.UnlockInstance || command.CompleteInstance)
                {
                    context.InstanceHandle.Free();
                }
            }

            return true;
        }


        private bool LoadWorkflow(InstancePersistenceContext context, LoadWorkflowCommand command)
        {
            if (command.AcceptUninitializedInstance)
            {
                return false;
            }

            if (context.InstanceVersion == -1)
            {
                context.BindAcquiredLock(0);
            }

            var package = _repository.Load(context.InstanceView.InstanceId);
            var data = Deserialize(package.Data);
            var metadata = Deserialize(package.Metadata);
            context.LoadedInstance(InstanceState.Initialized, data, metadata, null, null);
            return true;
        }


        private bool CreateWorkflowOwner(InstancePersistenceContext context, CreateWorkflowOwnerCommand command)
        {
            var instanceOwnerId = Guid.NewGuid();
            context.BindInstanceOwner(instanceOwnerId, instanceOwnerId);
            context.BindEvent(HasRunnableWorkflowEvent.Value);
            return true;
        }

        private bool DeleteWorkflowOwner(InstancePersistenceContext context, DeleteWorkflowOwnerCommand command)
        {
            return true;
        }

        #endregion

        #region serialization

        private IDictionary<XName, InstanceValue> Deserialize(IDictionary<string, InstanceValue> serializableInstanceData)
        {
            


            var destination = new Dictionary<XName, InstanceValue>();

            foreach (var property in serializableInstanceData!)
            {
                destination.Add(property.Key, property.Value);
            }

            return destination;
        }

        private  IDictionary<string, InstanceValue> SerializeData(IDictionary<XName, InstanceValue> source)
        {
            var scratch = new Dictionary<string, InstanceValue>();
            foreach (var property in source)
            {
                var writeOnly = (property.Value.Options & InstanceValueOptions.WriteOnly) != 0;

                if (!writeOnly && !property.Value.IsDeletedValue)
                {
                    scratch.Add(property.Key.ToString(), property.Value);
                }
            }

            return scratch;
        }

        private static IDictionary<string, InstanceValue> SerializeMetadata(IDictionary<XName, InstanceValue> states, IDictionary<XName, InstanceValue> changes)
        {
            IDictionary<string, InstanceValue> metadata = new Dictionary<string, InstanceValue>();
            foreach (var state in states)
            {
                metadata.Add(state.Key.ToString(), state.Value);
            }

            foreach (var change in changes)
            {
                if (change.Value.Options.HasFlag(InstanceValueOptions.WriteOnly)) continue;

                if (metadata.ContainsKey(change.Key.ToString()))
                {
                    if (change.Value.IsDeletedValue) metadata.Remove(change.Key.ToString());
                    else metadata[change.Key.ToString()] = change.Value;
                }
                else
                {
                    if (!change.Value.IsDeletedValue) metadata.Add(change.Key.ToString(), change.Value);
                }
            }

            return metadata;
        }

        #endregion

        #region private classes

        private abstract class AsyncResult : IAsyncResult
        {
            private readonly AsyncCallback _callback;
            private bool _completedSynchronously;
            private bool _endCalled;
            private Exception _exception;
            private bool _isCompleted;
            private ManualResetEvent _manualResetEvent;

            protected AsyncResult(AsyncCallback callback, object state)
            {
                _callback = callback;
                AsyncState = state;
                ThisLock = new object();
            }

            public object AsyncState { get; }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (_manualResetEvent != null)
                    {
                        return _manualResetEvent;
                    }

                    lock (ThisLock)
                    {
                        _manualResetEvent ??= new ManualResetEvent(_isCompleted);
                    }

                    return _manualResetEvent;
                }
            }

            public bool CompletedSynchronously => _completedSynchronously;

            public bool IsCompleted => _isCompleted;

            private object ThisLock { get; }

            // Call this version of complete when your asynchronous operation is complete.  This will update the state
            // of the operation and notify the callback.
            protected void Complete(bool completedSynchronously)
            {
                if (_isCompleted)
                {
                    // It is incorrect to call Complete twice.
                    // throw new InvalidOperationException(Resources.AsyncResultAlreadyCompleted);
                    throw new InvalidOperationException("AsyncResultAlreadyCompleted");
                }

                _completedSynchronously = completedSynchronously;

                if (completedSynchronously)
                {
                    // If we completedSynchronously, then there is no chance that the manualResetEvent was created so
                    // we do not need to worry about a race condition.
                    Debug.Assert(_manualResetEvent == null,
                        "No ManualResetEvent should be created for a synchronous AsyncResult.");
                    _isCompleted = true;
                }
                else
                {
                    lock (ThisLock)
                    {
                        _isCompleted = true;
                        _manualResetEvent?.Set();
                    }
                }

                // If the callback throws, the callback implementation is incorrect
                _callback?.Invoke(this);
            }

            // Call this version of complete if you raise an exception during processing.  In addition to notifying
            // the callback, it will capture the exception and store it to be thrown during AsyncResult.End.
            protected void Complete(bool completedSynchronously, Exception exception)
            {
                _exception = exception;
                Complete(completedSynchronously);
            }

            // End should be called when the End function for the asynchronous operation is complete.  It
            // ensures the asynchronous operation is complete, and does some common validation.
            protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
                where TAsyncResult : AsyncResult
            {
                if (result == null)
                {
                    throw new ArgumentNullException(nameof(result));
                }


                if (!(result is TAsyncResult asyncResult))
                {
                    // throw new ArgumentException(Resources.InvalidAsyncResult);
                    throw new ArgumentException("InvalidAsyncResult");
                }

                if (asyncResult._endCalled)
                {
                    // throw new InvalidOperationException(Resources.AsyncResultAlreadyEnded);
                    throw new InvalidOperationException("AsyncResultAlreadyEnded");
                }

                asyncResult._endCalled = true;

                if (!asyncResult._isCompleted)
                {
                    asyncResult.AsyncWaitHandle.WaitOne();
                }

                // was manualResetEvent.Close();
                asyncResult._manualResetEvent?.Dispose();

                if (asyncResult._exception != null)
                {
                    throw asyncResult._exception;
                }

                return asyncResult;
            }
        }


        /// <summary>
        /// A strongly typed AsyncResult.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private abstract class TypedAsyncResult<T> : AsyncResult
        {
            private T _data;

            protected TypedAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            public T Data => _data;

            protected void Complete(T data, bool completedSynchronously)
            {
                _data = data;
                Complete(completedSynchronously);
            }

            protected static T End(IAsyncResult result)
            {
                TypedAsyncResult<T> typedResult = AsyncResult.End<TypedAsyncResult<T>>(result);
                return typedResult.Data;
            }
        }

        private class TypedCompletedAsyncResult<T> : TypedAsyncResult<T>
        {
            public TypedCompletedAsyncResult(T data, AsyncCallback callback, object state)
                : base(callback, state)
            {
                Complete(data, true);
            }

            public new static T End(IAsyncResult result)
            {
                if (!(result is TypedCompletedAsyncResult<T> completedResult))
                {
                    throw new ArgumentException("InvalidAsyncResult");
                }

                return TypedAsyncResult<T>.End(completedResult);
            }
        }

        #endregion
    }
}