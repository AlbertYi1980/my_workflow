namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public interface IDescriptorProvider
    {
        IActivityDescriptor Find(string name);
    }
}