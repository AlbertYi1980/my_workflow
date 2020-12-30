using System.Activities;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Special;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class TraceDescriptor : IActivityDescriptor
    {
        public string Name => "trace";
        
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var trace = new Trace
            {
                DisplayName = ParseHelper.GetDisplayName(node),
                Text = new CSharpValue<string>(node.GetProperty("text").GetString())
            };
            return trace;
        }
    }
}