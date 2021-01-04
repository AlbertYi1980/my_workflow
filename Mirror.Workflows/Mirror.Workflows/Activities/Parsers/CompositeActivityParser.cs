using System;
using System.Activities;
using System.Text.Json;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers
{
    public interface IMyActivityDescriptor
    {
        string Name { get; }
        Activity Parse(JsonElement node);
    }

    
    public abstract class ActivityDescriptorBase : IMyActivityDescriptor
    {
        private readonly CompositeActivityParser _compositeActivityParser;
        public string Name { get; }
        protected ITypeInfoProvider TypeInfoProvider { get; }

        protected ActivityDescriptorBase(ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeActivityParser, string name)
        {
            _compositeActivityParser = compositeActivityParser;
            TypeInfoProvider = typeInfoProvider;
            Name = name;
        }
        
        public Activity Parse(JsonElement node)
        {
            throw new NotImplementedException();
        }
    }


    public class CompositeActivityParser
    {
        private readonly IDescriptorProvider _descriptorProvider;
        private readonly ITypeInfoProvider _typeInfoProvider;
        
        public CompositeActivityParser(IDescriptorProvider descriptorProvider, ITypeInfoProvider typeInfoProvider)
        {
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _typeInfoProvider = typeInfoProvider ?? throw new ArgumentNullException(nameof(typeInfoProvider));
        }

      

        internal Activity Parse(JsonElement node)
        {
            var typeName = node.GetProperty("$type").GetString();
            var descriptor = _descriptorProvider.Find(typeName);
            if (descriptor == null)
            {
                throw new Exception($"can not find descriptor of activity type {typeName}");
            }
            return descriptor.Parse(node, _typeInfoProvider, this);
        }
        
  
    }
}