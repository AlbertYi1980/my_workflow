using System.Activities;
using System.Activities.Statements;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.Activities.Parsers.Descriptors.WellKnown
{
    public class AssignDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _coreMethod;

        public AssignDescriptor()
        {
            _coreMethod = typeof(AssignDescriptor).GetMethod(nameof(ParseAssignCore),
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public string Name => "assign";

        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var typeName = node.GetProperty("type").GetString();
            var type = typeInfoProvider.Find(typeName);
            var coreMethod = _coreMethod!.MakeGenericMethod(type);
            return (Activity) coreMethod.Invoke(this, new object[] {node});
        }

        private Activity ParseAssignCore<T>(JsonElement node)
        {
            var assign = new Assign<T>()
            {
                DisplayName = ActivityParseUtil.GetDisplayName(node)
            };
            var to = node.GetProperty("to").GetString();
            assign.To = new OutArgument<T>(new CSharpReference<T>(to));
            var value = node.GetProperty("value").GetString();
            assign.Value = new CSharpValue<T>(value);
            return assign;
        }
    }
}