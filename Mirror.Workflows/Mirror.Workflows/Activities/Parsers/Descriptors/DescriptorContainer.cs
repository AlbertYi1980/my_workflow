using System.Collections.Concurrent;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public sealed class DescriptorContainer : IDescriptorProvider
    {
        private readonly IDescriptorProvider _parent;

        private readonly ConcurrentDictionary<string, IActivityDescriptor> _descriptors =
            new ConcurrentDictionary<string, IActivityDescriptor>();

        public DescriptorContainer(IDescriptorProvider parent)
        {
            _parent = parent;
        }

        public bool Add(IActivityDescriptor descriptor)
        {
            return _descriptors.TryAdd(descriptor.Name, descriptor);
        }

        public IActivityDescriptor Find(string name)
        {
            var descriptor = _parent?.Find(name);
            if (descriptor != null) return descriptor;
            return _descriptors.TryGetValue(name, out  descriptor) ? descriptor : null;
        }
    }
}