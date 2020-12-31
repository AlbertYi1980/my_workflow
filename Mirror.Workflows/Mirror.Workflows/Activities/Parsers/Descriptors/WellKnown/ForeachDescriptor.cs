using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class ForeachDescriptor : IActivityDescriptor
    {
        public string Name => "foreach";

        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var values = node.GetProperty("values").GetString();
            var valueName = node.GetProperty("valueName").GetString();
            var bodyExists = node.TryGetProperty("body", out var bodyNode);

            var @foreach = new ForEach<JsonElement>
            {
                DisplayName = ActivityParseUtil. GetDisplayName(node),
                Values = new CSharpValue<IEnumerable<JsonElement>>(values)
            };

            var activityAction = new ActivityAction<JsonElement>
            {
                Argument = new DelegateInArgument<JsonElement>(valueName)
            };
            if (bodyExists)
            {
                activityAction.Handler = compositeParser. Parse(bodyNode);
            }

            @foreach.Body = activityAction;
            return @foreach;
        }
    }
}