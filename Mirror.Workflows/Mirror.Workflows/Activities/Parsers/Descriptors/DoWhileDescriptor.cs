using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class DoWhileDescriptor : IActivityDescriptor
    {
        public string Name => "doWhile";
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var doWhile = new DoWhile
            {
                DisplayName =  ParseHelper. GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString())
            };
            var bodyExists = node.TryGetProperty("body", out var bodyNode);
            if (bodyExists)
            {
                doWhile.Body = context.CompositeParser.ParseActivity(bodyNode);
            }

            foreach (var v in ParseHelper. ParseVariables(node, context.TypeContainer))
            {
                doWhile.Variables.Add(v);
            }

            return doWhile;
        }
    }
}