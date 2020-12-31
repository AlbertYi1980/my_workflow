using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class IfDescriptor : IActivityDescriptor
    {
        public string Name => "if";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var thenNodeExists = node.TryGetProperty("then", out var thenNode);
            var elseNodeExists = node.TryGetProperty("else", out var elseNode);

            var @if = new If
            {
                DisplayName = ActivityParseUtil.GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString()),
            };

            if (thenNodeExists)
            {
                @if.Then =  compositeParser.Parse(thenNode);
            }
            if (elseNodeExists)
            {
                @if.Else = compositeParser.Parse(elseNode);
            }
            return @if;
        }
    }
}