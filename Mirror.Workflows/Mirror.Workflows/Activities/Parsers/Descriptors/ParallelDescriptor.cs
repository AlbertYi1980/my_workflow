using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class ParallelDescriptor : IActivityDescriptor
    {
 
        public string Name => "parallel";

        
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var displayName = ParseHelper. GetDisplayName(node);
            var branchesNodeExists = node.TryGetProperty("branches", out var branchesNode);
            node.TryGetProperty("completionCondition", out var completionConditionProperty);
            var parallel = new Parallel()
            {
                DisplayName = displayName,
                CompletionCondition = new CSharpValue<bool>(completionConditionProperty.GetString()),
            };
            foreach (var variable in ParseHelper. ParseVariables(node, context.TypeContainer))
            {
                parallel.Variables.Add(variable);
            }

            if (branchesNodeExists)
            {
                foreach (var branchNode in branchesNode.EnumerateArray())
                {
                    parallel.Branches.Add(context.CompositeParser.ParseActivity(branchNode));
                }
            }

            return parallel;
        }
    }
}