using System;

namespace Mirror.Workflows.InstanceStoring
{
    internal class TypedCompletedAsyncResult<T> : TypedAsyncResult<T>
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
}