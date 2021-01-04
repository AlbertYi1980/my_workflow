using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;

namespace Mirror.Workflows.InstanceStoring
{
    public class InstanceDataPackage
    {
        public InstanceDataPackage( IDictionary<string, InstanceValue> metadata,  IDictionary<string, InstanceValue> data)
        {
            Metadata = metadata;
            Data = data;
        }

        public  IDictionary<string, InstanceValue> Metadata { get; }
        public  IDictionary<string, InstanceValue> Data { get; }
    }
}