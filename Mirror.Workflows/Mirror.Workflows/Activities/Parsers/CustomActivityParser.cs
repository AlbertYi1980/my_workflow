using System;
using System.Activities;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers
{
    public class CustomActivityParser
    {
        private readonly CompositeActivityParser _compositeActivityParser;
        private readonly ITypeInfoProvider _typeInfoProvider;

        public CustomActivityParser(IDescriptorProvider descriptorProvider, ITypeInfoProvider typeInfoProvider)
        {
            _compositeActivityParser = new CompositeActivityParser(descriptorProvider, typeInfoProvider);
            _typeInfoProvider = typeInfoProvider ?? throw new ArgumentNullException(nameof(typeInfoProvider));
        }

        public Activity Parse(JsonElement node)
        {
            var implementation = node.GetProperty("implementation");
            var innerActivity = _compositeActivityParser.Parse(implementation);


            var outerActivity = new DynamicActivity()
            {
                Name = "jkjkj",
                DisplayName = "jkj",
                Attributes = { },
                Constraints = { },
                Implementation = () => innerActivity,
            };
            foreach (var argument in ParseArguments(node))
            {
                outerActivity.Properties.Add(argument);
            }

            var result = ParseResult(node);
            if (result != null)
            {
                outerActivity.Properties.Add(result);
            }
            return outerActivity;
        }

        private IEnumerable<DynamicActivityProperty> ParseArguments(JsonElement node)
        {
            var exists = node.TryGetProperty("arguments", out var argumentsNode);
            if (!exists) yield break;
            foreach (var argumentNode in argumentsNode.EnumerateArray())
            {
                var typeName = argumentNode.GetProperty("type").GetString();
                var name = argumentNode.GetProperty("name").GetString();
                var type = _typeInfoProvider.Find(typeName);
                var value = Argument.Create(type, ArgumentDirection.In);
                yield return new DynamicActivityProperty
                {
                    Name = name,
                    Type = value.GetType(),
                    Value = value
                };
            }
        }

        private DynamicActivityProperty ParseResult(JsonElement node)
        {
            var exists = node.TryGetProperty("result", out var resultNode);
            if (!exists) return null;
            var typeName = resultNode.GetProperty("type").GetString();
            var name = "result";
            var type = _typeInfoProvider.Find(typeName);
            var value = Argument.Create(type, ArgumentDirection.Out);
            return new DynamicActivityProperty
            {
                Name = name,
                Type = value.GetType(),
                Value = value
            };
        }

  
    }
}