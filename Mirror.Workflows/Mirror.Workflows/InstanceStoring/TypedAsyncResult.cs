using System;

namespace Mirror.Workflows.InstanceStoring
{
    /// <summary>
    /// A strongly typed AsyncResult.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class TypedAsyncResult<T> : AsyncResult
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
}