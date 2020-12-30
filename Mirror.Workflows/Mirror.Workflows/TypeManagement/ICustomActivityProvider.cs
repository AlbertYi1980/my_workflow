using System.Collections.Generic;
using Mirror.Workflows.Activities;

namespace Mirror.Workflows.TypeManagement
{
    public interface ICustomActivityProvider
    {
        IEnumerable<ActivityDescriptor> GetDescriptors();
    }
}