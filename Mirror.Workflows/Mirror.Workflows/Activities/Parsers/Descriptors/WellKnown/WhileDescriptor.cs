using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class WhileDescriptor : IActivityDescriptor
    {
        public string Name => "while";
        
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var @while = new While
            {
                DisplayName = ActivityParseUtil.GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString())
            };
            var bodyExists = node.TryGetProperty("body", out var bodyNode);
            if (bodyExists)
            {
                @while.Body = compositeParser.Parse(bodyNode);
            }

            foreach (var v in ActivityParseUtil.ParseVariables(node, typeInfoProvider))
            {
                @while.Variables.Add(v);
            }

            return @while;
        }
    }
}