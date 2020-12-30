using System.Collections.Generic;
using Mirror.Workflows.Activities;
using Mirror.Workflows.Activities.Parsers;

namespace Mirror.Workflows.TypeManagement
{
    public interface ICustomActivityProvider
    {
        IEnumerable<ActivityDescriptor> GetDescriptors();
    }
}