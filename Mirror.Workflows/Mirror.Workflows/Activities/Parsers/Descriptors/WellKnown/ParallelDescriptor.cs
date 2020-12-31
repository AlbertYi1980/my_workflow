using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class ParallelDescriptor : IActivityDescriptor
    {
 
        public string Name => "parallel";

        
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var displayName = ActivityParseUtil. GetDisplayName(node);
            var branchesNodeExists = node.TryGetProperty("branches", out var branchesNode);
            node.TryGetProperty("completionCondition", out var completionConditionProperty);
            var parallel = new Parallel()
            {
                DisplayName = displayName,
                CompletionCondition = new CSharpValue<bool>(completionConditionProperty.GetString()),
            };
            foreach (var variable in ActivityParseUtil. ParseVariables(node, typeInfoProvider))
            {
                parallel.Variables.Add(variable);
            }

            if (branchesNodeExists)
            {
                foreach (var branchNode in branchesNode.EnumerateArray())
                {
                    parallel.Branches.Add(compositeParser.Parse(branchNode));
                }
            }

            return parallel;
        }
    }
}