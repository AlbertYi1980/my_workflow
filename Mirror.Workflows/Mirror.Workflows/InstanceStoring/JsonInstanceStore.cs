using System;
using System.Activities.DurableInstancing;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;
// ReSharper disable UnusedParameter.Local

namespace Mirror.Workflows.InstanceStoring
{
    public class JsonInstanceStore : InstanceStore
    {
        private readonly IInstanceRepository _repository;

        public JsonInstanceStore(IInstanceRepository repository)
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

        private IDictionary<XName, InstanceValue> Deserialize(string data)
        {
            var serializableInstanceData = JsonSerializer.Deserialize<Dictionary<string, InstanceValue>>(data);


            var destination = new Dictionary<XName, InstanceValue>();

            foreach (var property in serializableInstanceData!)
            {
                destination.Add(property.Key, property.Value);
            }

            return destination;
        }

        private string SerializeData(IDictionary<XName, InstanceValue> source)
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

            return JsonSerializer.Serialize(scratch);
        }

        private static string SerializeMetadata(IDictionary<XName, InstanceValue> states, IDictionary<XName, InstanceValue> changes)
        {
            var metadata =  new Dictionary<string, InstanceValue>();
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

            return JsonSerializer.Serialize(metadata);
        }

        #endregion
    }
}