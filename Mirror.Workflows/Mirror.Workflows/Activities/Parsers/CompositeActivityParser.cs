using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.Activities.Special;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers
{
    public class CompositeActivityParser
    {
        private readonly IDescriptorProvider _descriptorProvider;
        private readonly TypeContainer _typeContainer;


        public CompositeActivityParser(IDescriptorProvider descriptorProvider, TypeContainer typeContainer)
        {
            _descriptorProvider = descriptorProvider;
            _typeContainer = typeContainer;
        }

        public Activity Parse(string definition)
        {
            return ParseActivity(JsonSerializer.Deserialize<JsonElement>(definition));
        }


        public Activity ParseActivity(JsonElement node)
        {
            var type = node.GetProperty("$type").GetString();
            var foo = _descriptorProvider.Find(type);

            if (foo == null)
            {
                throw new Exception($"can not find parser for activity type {type}");
            }

            return foo.Parse(node, new ParseContext(this, _typeContainer));
        }



      

    }
}