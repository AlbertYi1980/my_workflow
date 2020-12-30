using System;
using System.Activities;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public static class ParseHelper
    {
        public static string GetDisplayName(JsonElement node)
        {
            var exists = node.TryGetProperty("displayName", out var displayNameProperty);
            if (!exists) return null;
            return displayNameProperty.GetString();
        }
        
        public static IEnumerable<Variable> ParseVariables(JsonElement node, TypeContainer typeContainer)
        {
            var exists = node.TryGetProperty("variables", out var variablesNode);
            if (!exists) yield break;
            foreach (var variableNode in variablesNode.EnumerateArray())
            {
                var typeText = variableNode.GetProperty("type").GetString();
                var type = MapType(typeText, typeContainer);
                var coreMethod = typeof(CompositeActivityParser).GetMethod(nameof(ParseVariableCore),
                    BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
                coreMethod = coreMethod!.MakeGenericMethod(type);
                yield return (Variable) coreMethod.Invoke(null, new object[] {variableNode});
            }
        }
        
        private static Type MapType(string type, TypeContainer typeContainer)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
            switch (type)
            {
                case "int":
                    return typeof(int);
                case "string":
                    return typeof(string);
                case "json":
                    return typeof(string);

                default:
                    var result = typeContainer.FindType(type);
                    if (result == null)
                    {
                        throw new NotSupportedException($"can not support type {type}");
                    }

                    return result;
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
                variable.Default = new CSharpValue<T>(defaultExpression);
            }

            return variable;
        }
    }
}