using System;
using System.Activities;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers
{
    public static class ActivityParseUtil
    {
        public static string GetDisplayName(JsonElement node)
        {
            var exists = node.TryGetProperty("displayName", out var displayNameProperty);
            if (!exists) return null;
            return displayNameProperty.GetString();
        }
        
        public static IEnumerable<Variable> ParseVariables(JsonElement node, ITypeInfoProvider typeInfoProvider)
        {
            var exists = node.TryGetProperty("variables", out var variablesNode);
            if (!exists) yield break;
            foreach (var variableNode in variablesNode.EnumerateArray())
            {
                var typeName = variableNode.GetProperty("type").GetString();
                var type = typeInfoProvider.Find(typeName);
                if (type == null) throw new Exception($"can not find type {typeName}");
                var coreMethod = typeof(ActivityParseUtil).GetMethod(nameof(ParseVariableCore),
                    BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic);
                coreMethod = coreMethod!.MakeGenericMethod(type);
                yield return (Variable) coreMethod.Invoke(null, new object[] {variableNode});
            }
        }

        private static Variable ParseVariableCore<T>(JsonElement variableNode)
        {
            var name = variableNode.GetProperty("name").GetString();
            var defaultNodeExists = variableNode.TryGetProperty("default", out var defaultNode);
            var variable = Variable.Create(name, typeof(T), VariableModifiers.None);
            if (defaultNodeExists)
            {
                var defaultExpression = defaultNode.GetString();
                variable.Default =   new CSharpValue<T>(defaultExpression);
            }
            return variable;
        }
    }
}