using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class PickDescriptor : IActivityDescriptor
    {
      
        public string Name => "pick";
        
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var pick = new Pick {DisplayName = ActivityParseUtil. GetDisplayName(node)};
            var branchesNodeExists = node.TryGetProperty("branches", out var branchesNode);
            if (branchesNodeExists)
            {
                foreach (var branchNode in branchesNode.EnumerateArray())
                {
                    pick.Branches.Add(ParsePickBranch(branchNode, typeInfoProvider, compositeParser));
                }
            }
            return pick;
        }
        
        private PickBranch ParsePickBranch(JsonElement branchNode,  ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var displayName = ActivityParseUtil. GetDisplayName(branchNode);
            var triggerNode = branchNode.GetProperty("trigger");
            var actionNodeExists = branchNode.TryGetProperty("action", out var actionNode);
            var branch = new PickBranch
            {
                DisplayName = displayName,
                Trigger = compositeParser. Parse(triggerNode),
                Action = actionNodeExists ? null : compositeParser. Parse(actionNode),
            };
            foreach (var variable in ActivityParseUtil.ParseVariables(branchNode, typeInfoProvider))
            {
                branch.Variables.Add(variable);
            }
            return branch;
        }
    }
}