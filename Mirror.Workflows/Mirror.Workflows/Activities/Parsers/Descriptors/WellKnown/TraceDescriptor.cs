using System.Activities;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Specials;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class TraceDescriptor : IActivityDescriptor
    {
        public string Name => "trace";
        
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var trace = new Trace
            {
                DisplayName = ActivityParseUtil.GetDisplayName(node),
                Text = new CSharpValue<string>(node.GetProperty("text").GetString())
      
            };
            return trace;
        }
    }
}