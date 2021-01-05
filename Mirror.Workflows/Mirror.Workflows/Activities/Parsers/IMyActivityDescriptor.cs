using System.Activities;
using System.Text.Json;

namespace Mirror.Workflows.Activities.Parsers
{
    public interface IMyActivityDescriptor
    {
        string Name { get; }
        Activity Parse(JsonElement node);
    }
}