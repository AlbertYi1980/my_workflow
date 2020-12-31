using System;
using System.Activities;
using System.Activities.Statements;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.Activities.Specials;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers
{
    public class CompositeActivityParser
    {
        private readonly IDescriptorProvider _descriptorProvider;
        private readonly ITypeInfoProvider _typeInfoProvider;
        
        public CompositeActivityParser(IDescriptorProvider descriptorProvider, ITypeInfoProvider typeInfoProvider)
        {
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _typeInfoProvider = typeInfoProvider ?? throw new ArgumentNullException(nameof(typeInfoProvider));
        }

        public Activity Parse(string definition)
        {
            return Parse(JsonSerializer.Deserialize<JsonElement>(definition));
        }

        public Activity Parse(JsonElement node)
        {
            var typeName = node.GetProperty("$type").GetString();
            var descriptor = _descriptorProvider.Find(typeName);
            if (descriptor == null)
            {
                throw new Exception($"can not find descriptor of activity type {typeName}");
            }
            return descriptor.Parse(node, _typeInfoProvider, this);
        }
        
        public Activity ParseCustom(JsonElement node)
        {
            var implementation = node.GetProperty("implementation");
            var innerActivity = Parse(implementation);
            var a1 = new Sequence()
            {
                Activities = { new Trace()
                {
                    Text = new CSharpValue<string>("\"kkkkkk\"")
                    // Text ="dfdddddddddddddddddddd"
                }}
            };
            // return a1;
            return new DynamicActivity()
            {
                Name = "jkjkj",
                DisplayName = "jkj",
                ImplementationVersion = new Version(1, 2),
                Attributes = { },
                Constraints = { },
                Properties = { },
                
                Implementation = () => a1,
          
            };
        }
    }
}