using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class PickDescriptor : IActivityDescriptor
    {
      
        public string Name => "pick";

    
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var pick = new Pick {DisplayName = ParseHelper. GetDisplayName(node)};
            var branchesNodeExists = node.TryGetProperty("branches", out var branchesNode);
            if (branchesNodeExists)
            {
                foreach (var branchNode in branchesNode.EnumerateArray())
                {
                    pick.Branches.Add(ParsePickBranch(branchNode, context));
                }
            }

            return pick;
        }
        
        private PickBranch ParsePickBranch(JsonElement branchNode, ParseContext context)
        {
            var displayName = ParseHelper. GetDisplayName(branchNode);
            var triggerNode = branchNode.GetProperty("trigger");
            var actionNodeExists = branchNode.TryGetProperty("action", out var actionNode);
            var branch = new PickBranch
            {
                DisplayName = displayName,
                Trigger = context.CompositeParser. ParseActivity(triggerNode),
                Action = actionNodeExists ? null : context.CompositeParser. ParseActivity(actionNode),
            };
            foreach (var variable in ParseHelper.ParseVariables(branchNode, context.TypeContainer))
            {
                branch.Variables.Add(variable);
            }

            return branch;
        }
    }
}