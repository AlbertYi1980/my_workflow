using System;
using System.Activities;
using System.Text.Json;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers
{
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
}