using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class WriteLineDescriptor : IActivityDescriptor
    {
        public string Name => "writeLine";
        
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var writeLine = new WriteLine
            {
                DisplayName =  ActivityParseUtil.GetDisplayName(node),
                Text = new CSharpValue<string>(node.GetProperty("text").GetString())
            };
            return writeLine;
        }
    }
}