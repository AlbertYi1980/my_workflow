using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class WhileDescriptor : IActivityDescriptor
    {
  
        public string Name => "while";
        
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var @while = new While
            {
                DisplayName = ParseHelper.GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString())
            };
            var bodyExists = node.TryGetProperty("body", out var bodyNode);
            if (bodyExists)
            {
                @while.Body = context.CompositeParser.ParseActivity(bodyNode);
            }

            foreach (var v in ParseHelper.ParseVariables(node, context.TypeContainer))
            {
                @while.Variables.Add(v);
            }

            return @while;
        }
    }
}