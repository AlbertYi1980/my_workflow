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
        private MethodInfo _coreMethod;

        public SwitchDescriptor()
        {
            _coreMethod = typeof(SwitchDescriptor).GetMethod(nameof(ParseSwitchCore),
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public string Name => "switch";

        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var typeName = node.GetProperty("type").GetString();
            var type = typeInfoProvider.Find(typeName);
            var coreMethod = _coreMethod!.MakeGenericMethod(type);
            return (Activity) coreMethod.Invoke(this, new object[] {node, compositeParser});

        }
        
        private Activity ParseSwitchCore<T>(JsonElement node, CompositeActivityParser compositeParser)
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
                foreach (var caseNode in casesNode.EnumerateArray())
                {
                    var key = caseNode.GetProperty("key").GetString();
                    var value = compositeParser. Parse(caseNode.GetProperty("value"));
                    @switch.Cases.Add(new KeyValuePair<T, Activity>(ValueFromText<T>(key), value));
                }
            }
            return @switch;
        }
        
        private T ValueFromText<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(int)) return (T) (object) int.Parse(key);
            if (type == typeof(string)) return (T) (object) (key);
            if (type == typeof(bool)) return (T) (object) bool.Parse(key);
            throw new NotSupportedException($"not support type {type.Name}");
        }
    }
}