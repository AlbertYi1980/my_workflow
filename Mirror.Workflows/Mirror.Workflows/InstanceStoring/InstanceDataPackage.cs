namespace Mirror.Workflows.InstanceStoring
{
    public class InstanceDataPackage
    {
        public InstanceDataPackage(string metadata, string data)
        {
            Metadata = metadata;
            Data = data;
        }

        public string Metadata { get; }
        public string Data { get; }
    }
}