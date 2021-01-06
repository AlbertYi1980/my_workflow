using System.Activities;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.CustomActivities.Scripts;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.CustomActivities.Descriptors
{
    public class CSharpDescriptor : IActivityDescriptor
    {
        private MethodInfo _methodInfo;

        public CSharpDescriptor()
        {
            _methodInfo = typeof(CSharpDescriptor).GetMethod(nameof(CreateNode), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string Name => "c#";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider,
            CompositeActivityParser compositeParser)
        {
            var inputType = typeInfoProvider.Find(node.GetProperty("inType").GetString());
            var outputType = typeInfoProvider.Find(node.GetProperty("outType").GetString());
            var argumentsExits = node.TryGetProperty("arguments", out var argumentsNode);
            var arguments = argumentsExits ? argumentsNode.GetString() : null;
            var code = node.GetProperty("code").GetString();
            var displayName = ActivityParseUtil.GetDisplayName(node);
            var result = node.GetProperty("result").GetString();
             var methodInfo = _methodInfo!.MakeGenericMethod(inputType, outputType);
            return (Activity)methodInfo.Invoke(this, new object[] {displayName, code, result, arguments});
        }

        private Activity CreateNode<TIn, TOut>(string displayName, string code, string result, string arguments)
        {
            var csharp = new CSharpScript<TIn, TOut>
            {
                DisplayName =  displayName,
                Code = code,
                Arguments = new InArgument<TIn>(new CSharpValue<TIn>(arguments)),
                Result = new OutArgument<TOut>(new CSharpReference<TOut>(result))
            };
            return csharp;
        }
    }
}