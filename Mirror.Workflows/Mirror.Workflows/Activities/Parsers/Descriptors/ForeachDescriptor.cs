using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Microsoft.VisualBasic;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class ForeachDescriptor : IActivityDescriptor
    {
       
        public string Name => "foreach";

      
        
        public Activity Parse(JsonElement node, ParseContext context)
        {
            var values = node.GetProperty("values").GetString();
            var valueName = node.GetProperty("valueName").GetString();
            var bodyExists = node.TryGetProperty("body", out var bodyNode);

            var @foreach = new ForEach<JsonElement>
            {
                DisplayName = ParseHelper. GetDisplayName(node),
                Values = new CSharpValue<IEnumerable<JsonElement>>(values)
            };

            var activityAction = new ActivityAction<JsonElement>
            {
                Argument = new DelegateInArgument<JsonElement>(valueName)
            };
            if (bodyExists)
            {
                activityAction.Handler = context.CompositeParser. ParseActivity(bodyNode);
            }

            @foreach.Body = activityAction;
            return @foreach;
        }
    }
}