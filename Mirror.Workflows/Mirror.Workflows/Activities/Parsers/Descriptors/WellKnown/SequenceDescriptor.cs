using System.Activities;
using System.Activities.Statements;
using System.Linq;
using System.Text.Json;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class SequenceDescriptor : IActivityDescriptor
    {
      
        public string Name => "sequence";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var activitiesNodeExists = node.TryGetProperty("activities", out var activitiesNode);

            var sequence = new Sequence
            {
                DisplayName =  ActivityParseUtil.GetDisplayName(node),
            };
            if (activitiesNodeExists)
            {
                var activities = activitiesNode.EnumerateArray()
                    .Select(compositeParser.Parse);
                foreach (var activity in activities)
                {
                    sequence.Activities.Add(activity);
                }
            }

            foreach (var v in ActivityParseUtil.ParseVariables(node, typeInfoProvider))
            {
                sequence.Variables.Add(v);
            }

            return sequence;
        }
    }
}