using System.Activities;
using System.Activities.Statements;
using System.Linq;
using System.Text.Json;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class SequenceDescriptor : IActivityDescriptor
    {
      
        public string Name => "sequence";
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var activitiesNodeExists = node.TryGetProperty("activities", out var activitiesNode);

            var sequence = new Sequence
            {
                DisplayName =  ParseHelper.GetDisplayName(node),
            };
            if (activitiesNodeExists)
            {
                var activities = activitiesNode.EnumerateArray()
                    .Select(context.CompositeParser.ParseActivity);
                foreach (var activity in activities)
                {
                    sequence.Activities.Add(activity);
                }
            }

            foreach (var v in ParseHelper.ParseVariables(node, context.TypeContainer))
            {
                sequence.Variables.Add(v);
            }

            return sequence;
        }
    }
}