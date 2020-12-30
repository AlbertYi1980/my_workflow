using System.Activities;
using System.Text.Json;
using Mirror.Workflows.Activities.Parsers.Descriptors;

namespace Mirror.Workflows.Activities.Parsers
{
    public interface IActivityDescriptor
    {
        string Name { get; }
        Activity Parse(JsonElement node, ParseContext context);
    }
}