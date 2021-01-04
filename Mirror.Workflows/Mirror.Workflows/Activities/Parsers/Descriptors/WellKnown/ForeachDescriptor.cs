using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class ForeachDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _coreMethod;
        public string Name => "foreach";

        public ForeachDescriptor()
        {
            _coreMethod = typeof(ForeachDescriptor).GetMethod(nameof(ParseCore),
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var typeName = node.GetProperty("type").GetString();
            var type = typeInfoProvider.Find(typeName);
            if (type == null) throw new Exception($"can not find type name {typeName}");
            var coreMethod = _coreMethod!.MakeGenericMethod(type);
            return (Activity) coreMethod!.Invoke(this, new object[] {node,  compositeParser});
        }

        private Activity ParseCore<T>(JsonElement node, CompositeActivityParser compositeParser)
        {
            var values = node.GetProperty("values").GetString();
            var valueName = node.GetProperty("valueName").GetString();
            var bodyExists = node.TryGetProperty("body", out var bodyNode);

            var @foreach = new ForEach<T>
            {
                DisplayName = ActivityParseUtil.GetDisplayName(node),
                Values = new CSharpValue<IEnumerable<T>>(values)
            };

            var activityAction = new ActivityAction<T>
            {
                Argument = new DelegateInArgument<T>(valueName)
            };
            if (bodyExists)
            {
                activityAction.Handler = compositeParser.Parse(bodyNode);
            }

            @foreach.Body = activityAction;
            return @foreach;
        }
    }
}