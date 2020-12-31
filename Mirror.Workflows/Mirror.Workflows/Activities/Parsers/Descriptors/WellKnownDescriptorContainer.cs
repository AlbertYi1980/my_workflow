using System.Collections.Generic;
using Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public sealed class WellKnownDescriptorContainer : IDescriptorProvider
    {
        private readonly Dictionary<string, IActivityDescriptor> _descriptors =
            new Dictionary<string, IActivityDescriptor>();

        public WellKnownDescriptorContainer()
        {
            var wellKnownDescriptors = new IActivityDescriptor[]
            {
                new SequenceDescriptor(),
                new IfDescriptor(),
                new WhileDescriptor(),
                new DoWhileDescriptor(),
                new WriteLineDescriptor(),
                new TraceDescriptor(),
                new UserTaskDescriptor(),
                new SwitchDescriptor(),
                new ForeachDescriptor(),
                new AssignDescriptor(),
                new PickDescriptor(),
                new ParallelDescriptor(),
                new StateMachineDescriptor(),
            };
            foreach (var wellKnownDescriptor in wellKnownDescriptors)
            {
                _descriptors.Add(wellKnownDescriptor.Name, wellKnownDescriptor);
            }
        }

        public IActivityDescriptor Find(string name)
        {
            return _descriptors.TryGetValue(name, out var descriptor) ? descriptor : null;
        }
    }
}