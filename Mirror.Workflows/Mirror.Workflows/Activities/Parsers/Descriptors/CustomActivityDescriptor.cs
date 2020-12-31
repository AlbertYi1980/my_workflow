using System.Activities;
using System.Text.Json;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class  CustomActivityDescriptor : IActivityDescriptor
    {
        private readonly string _definition;

        public CustomActivityDescriptor(string name, string definition)
        {
            _definition = definition;
            Name = name;
        }
        public string Name { get;  }
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            return compositeParser.Parse(_definition);
        }
    }
}