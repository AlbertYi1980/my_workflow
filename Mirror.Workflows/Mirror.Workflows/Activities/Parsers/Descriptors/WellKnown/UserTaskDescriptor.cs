using System.Activities;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Specials;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class UserTaskDescriptor : IActivityDescriptor
    {
        public string Name => "userTask";
        
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var resultExists = node.TryGetProperty("result", out var resultNode);
            var userTask = new UserTask()
            {
                DisplayName = ActivityParseUtil.GetDisplayName(node),
                Name = node.GetProperty("name").GetString(),
            };
            if (resultExists)
            {
                userTask.Result = new OutArgument<string>(new CSharpReference<string>(resultNode.GetString()));
            }

            return userTask;
        }
    }
}