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
    public class MirrorDetailDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _methodInfo;

        public MirrorDetailDescriptor()
        {
            _methodInfo = typeof(MirrorDetailDescriptor).GetMethod(nameof(CreateNode),
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string  Name => "mirrorDetail";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var mirrorBase = node.GetProperty("mirrorBase").GetString();
            var tenantId = node.GetProperty("tenantId").GetString();
            var modelKey = node.GetProperty("modelKey").GetString();
            var modelType = typeInfoProvider.Find(modelKey);
            var filter = node.GetProperty("filter").GetString();
            filter = MirrorExpressionHelper.BuildCSharpExpression(filter);
            var result = node.GetProperty("result").GetString();
            var methodInfo = _methodInfo!.MakeGenericMethod(modelType);
            return  (Activity)methodInfo.Invoke(this, new object[] { mirrorBase, tenantId, modelKey, filter, result });
        }

        private Activity CreateNode<TModel>(string mirrorBase, string tenantId, string modelKey, string filter, string result)
        {
            var mirrorCreate = new MirrorDetail<TModel>
            {
                MirrorBase =  new InArgument<string>(new CSharpValue<string>(mirrorBase)),
                TenantId = new InArgument<string>(new CSharpValue<string>(tenantId)),
                ModelKey = modelKey,
                Filter = new InArgument<string>(new CSharpValue<string>(filter)),
                Result = new OutArgument<TModel>(new CSharpReference<TModel>(result))
            };

            return mirrorCreate;
        }
    }
}