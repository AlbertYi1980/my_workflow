using System.Activities;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.CustomActivities.MirrorCrud;
using Mirror.Workflows.Types;

namespace Mirror.Workflows.CustomActivities.Descriptors
{
    public class MirrorCreateDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _methodInfo;

        public MirrorCreateDescriptor()
        {
            _methodInfo = typeof(MirrorCreateDescriptor).GetMethod(nameof(CreateNode),
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string  Name => "mirrorCreate";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var mirrorBase = node.GetProperty("mirrorBase").GetString();
            var tenantId = node.GetProperty("tenantId").GetString();
            var modelKey = node.GetProperty("modelKey").GetString();
            var modelType = typeInfoProvider.Find(modelKey);
            var model = node.GetProperty("model").GetString();
            var result = node.GetProperty("result").GetString();
            var methodInfo = _methodInfo!.MakeGenericMethod(modelType);
            return  (Activity)methodInfo.Invoke(this, new object[] { mirrorBase, tenantId, modelKey, model, result });
        }

        private Activity CreateNode<TModel>(string mirrorBase, string tenantId, string modelKey, string model, string result)
        {
            return new MirrorCreate<TModel>()
            {
                MirrorBase =  new InArgument<string>(new CSharpValue<string>(mirrorBase)),     
                TenantId = new InArgument<string>(new CSharpValue<string>(tenantId)),
                ModelKey = modelKey,
                Model = new InArgument<TModel>(new CSharpValue<TModel>(model)),
                Result = new OutArgument<TModel>(new CSharpReference<TModel>(result))
            };
        }
    }
}