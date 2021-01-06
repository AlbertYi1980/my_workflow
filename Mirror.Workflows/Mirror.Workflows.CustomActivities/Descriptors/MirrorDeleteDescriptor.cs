using System.Activities;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.CustomActivities.MirrorCrud;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.CustomActivities.Descriptors
{
    public class MirrorDeleteDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _methodInfo;

        public MirrorDeleteDescriptor()
        {
            _methodInfo = typeof(MirrorDeleteDescriptor).GetMethod(nameof(CreateNode),
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string  Name => "mirrorDelete";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var mirrorBase = node.GetProperty("mirrorBase").GetString();
            var tenantId = node.GetProperty("tenantId").GetString();
            var modelKey = node.GetProperty("modelKey").GetString();
            var filter = node.GetProperty("filter").GetString();
            filter = MirrorExpressionHelper.BuildCSharpExpression(filter);
            var result = node.GetProperty("result").GetString();
            return  (Activity)_methodInfo.Invoke(this, new object[] { mirrorBase, tenantId, modelKey, filter, result });
        }

        private Activity CreateNode(string mirrorBase, string tenantId, string modelKey, string filter, string result)
        {
            return new MirrorDelete()
            {
                MirrorBase =  new InArgument<string>(new CSharpValue<string>(mirrorBase)),
                TenantId = new InArgument<string>(new CSharpValue<string>(tenantId)),
                ModelKey = modelKey,
                Filter = new InArgument<string>(new CSharpValue<string>(filter)),
                Result = new OutArgument<bool>(new CSharpReference<bool>(result))
            };
        }
    }
}