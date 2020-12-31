using System.Activities;
using System.Text.Json;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public interface IActivityDescriptor
    {
        string Name { get; }
        Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser);
    }
}