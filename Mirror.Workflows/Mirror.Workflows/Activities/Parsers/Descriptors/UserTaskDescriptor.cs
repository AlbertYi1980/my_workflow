using System.Activities;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Special;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class UserTaskDescriptor : IActivityDescriptor
    {
        public string Name => "userTask";
        
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var resultExists = node.TryGetProperty("result", out var resultNode);
            var userTask = new UserTask()
            {
                DisplayName = ParseHelper.GetDisplayName(node),
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