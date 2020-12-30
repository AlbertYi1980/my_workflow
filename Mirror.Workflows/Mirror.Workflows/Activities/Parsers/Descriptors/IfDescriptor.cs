using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class IfDescriptor : IActivityDescriptor
    {
      
        public string Name => "if";
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var thenNodeExists = node.TryGetProperty("then", out var thenNode);
            var elseNodeExists = node.TryGetProperty("else", out var elseNode);

            var @if = new If
            {
                DisplayName = ParseHelper.GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString()),
            };

            if (thenNodeExists)
            {
                @if.Then =  context.CompositeParser.ParseActivity(thenNode);
            }
            if (elseNodeExists)
            {
                @if.Else = context.CompositeParser.ParseActivity(elseNode);
            }
            return @if;
        }
    }
}