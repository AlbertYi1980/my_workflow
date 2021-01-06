using System.Activities;
using System.Reflection;
using System.Text.Json;
using IronPython.Modules;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.CustomActivities.MirrorCrud;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.CustomActivities.Descriptors
{
    public class MirrorUpdateDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _methodInfo;

        public MirrorUpdateDescriptor()
        {
            _methodInfo = typeof(MirrorUpdateDescriptor).GetMethod(nameof(CreateNode),
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string  Name => "mirrorUpdate";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var mirrorBase = node.GetProperty("mirrorBase").GetString();
            var tenantId = node.GetProperty("tenantId").GetString();
            var modelKey = node.GetProperty("modelKey").GetString();
            var filter = node.GetProperty("filter").GetString();
            filter = MirrorExpressionHelper.BuildCSharpExpression(filter);
            var model = node.GetProperty("model").GetString();
            model = MirrorExpressionHelper.BuildCSharpExpression(model);
            var result = node.GetProperty("result").GetString();
            return  (Activity)_methodInfo.Invoke(this, new object[] { mirrorBase, tenantId, modelKey, filter, model, result });
        }

        private Activity CreateNode(string mirrorBase, string tenantId, string modelKey, string filter, string model, string result)
        {
            return new MirrorUpdate()
            {
                MirrorBase =  new InArgument<string>(new CSharpValue<string>(mirrorBase)),
                TenantId = new InArgument<string>(new CSharpValue<string>(tenantId)),
                ModelKey = modelKey,
                Filter = new InArgument<string>(new CSharpValue<string>(filter)),
                Model = new InArgument<string>(new CSharpValue<string>(model)),
                Result = new OutArgument<bool>(new CSharpReference<bool>(result))
            };
        }
    }
}