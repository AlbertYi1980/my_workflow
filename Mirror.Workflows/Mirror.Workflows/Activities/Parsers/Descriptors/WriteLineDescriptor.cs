using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class WriteLineDescriptor : IActivityDescriptor
    {
        public string Name => "writeLine";
        
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var writeLine = new WriteLine
            {
                DisplayName =  ParseHelper.GetDisplayName(node),
                Text = new CSharpValue<string>(node.GetProperty("text").GetString())
            };
            return writeLine;
        }
    }
}