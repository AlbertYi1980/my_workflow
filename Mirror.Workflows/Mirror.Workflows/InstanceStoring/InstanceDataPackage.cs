using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;

namespace Mirror.Workflows.InstanceStoring
{
    public class InstanceDataPackage
    {
        public InstanceDataPackage( Dictionary<string, InstanceValue> metadata,  Dictionary<string, InstanceValue> data)
        {
            Metadata = metadata;
            Data = data;
        }

        public  Dictionary<string, InstanceValue> Metadata { get; }
        public  Dictionary<string, InstanceValue> Data { get; }
    }
}