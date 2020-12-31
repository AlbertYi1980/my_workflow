using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class DoWhileDescriptor : IActivityDescriptor
    {
        public string Name => "doWhile";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var doWhile = new DoWhile
            {
                DisplayName =  ActivityParseUtil. GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString())
            };
            var bodyExists = node.TryGetProperty("body", out var bodyNode);
            if (bodyExists)
            {
                doWhile.Body = compositeParser.Parse(bodyNode);
            }

            foreach (var v in ActivityParseUtil. ParseVariables(node, typeInfoProvider))
            {
                doWhile.Variables.Add(v);
            }

            return doWhile;
        }
    }
}