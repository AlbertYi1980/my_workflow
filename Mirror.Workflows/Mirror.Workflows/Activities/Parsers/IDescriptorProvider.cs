namespace Mirror.Workflows.Activities.Parsers
{
    public interface IDescriptorProvider
    {
        IActivityDescriptor Find(string name);
    }
}