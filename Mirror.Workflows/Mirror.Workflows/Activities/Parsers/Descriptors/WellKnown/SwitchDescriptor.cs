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
    public class SwitchDescriptor : IActivityDescriptor
    {
        public string Name => "switch";

        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var typeArguments = node.GetProperty("switchType").GetString();
            var type = typeInfoProvider.Find(typeArguments);
            var coreMethod = typeof(CompositeActivityParser).GetMethod(nameof(ParseSwitchCore),
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod!.MakeGenericMethod(type);
            return (Activity) coreMethod.Invoke(this, new object[] {node, typeInfoProvider, compositeParser});

        }
        
        private T ParseCaseKey<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(int)) return (T) (object) int.Parse(key);
            if (type == typeof(string)) return (T) (object) (key);
            throw new NotSupportedException($"not support type {type.Name}");
        }

        private Activity ParseSwitchCore<T>(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var defaultNodeExists = node.TryGetProperty("default", out var defaultNode);
            var casesNodeExists = node.TryGetProperty("cases", out var casesNode);
            var @switch = new Switch<T>
            {
                DisplayName = ActivityParseUtil. GetDisplayName(node),
                Expression = new CSharpValue<T>(node.GetProperty("expression").GetString())
            };

            if (defaultNodeExists)
            {
                @switch.Default = compositeParser. Parse(defaultNode);
            }

            if (casesNodeExists)
            {
                foreach (JsonElement caseNode in casesNode.EnumerateArray())
                {
                    var key = caseNode.GetProperty("key").GetString();
                    var valueNode = caseNode.GetProperty("value");
                    @switch.Cases.Add(new KeyValuePair<T, Activity>(ParseCaseKey<T>(key), compositeParser. Parse(valueNode)));
                }
            }
            return @switch;
        }
    }
}