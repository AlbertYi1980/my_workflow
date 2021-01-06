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
    public class MirrorQueryDescriptor : IActivityDescriptor
    {
        private readonly MethodInfo _methodInfo;

        public MirrorQueryDescriptor()
        {
            _methodInfo = typeof(MirrorQueryDescriptor).GetMethod(nameof(CreateNode),
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string  Name => "mirrorQuery";
        public Activity Parse(JsonElement node, ITypeInfoProvider typeInfoProvider, CompositeActivityParser compositeParser)
        {
            var mirrorBase = node.GetProperty("mirrorBase").GetString();
            var tenantId = node.GetProperty("tenantId").GetString();
            var modelKey = node.GetProperty("modelKey").GetString();
            var modelType = typeInfoProvider.Find(modelKey);
            var filter =  node.GetProperty("filter").GetString();
            filter = MirrorExpressionHelper.BuildCSharpExpression(filter);
            var sort = node.GetProperty("sort").GetString();   
            sort = MirrorExpressionHelper.BuildCSharpExpression(sort);
            var result = node.GetProperty("result").GetString();
            var methodInfo = _methodInfo!.MakeGenericMethod(modelType);
            return  (Activity)methodInfo.Invoke(this, new object[] { mirrorBase, tenantId, modelKey, filter, sort, result });
        }

        private Activity CreateNode<TModel>(string mirrorBase, string tenantId, string modelKey, string filter, string sort, string result)
        {
            return new MirrorQuery<TModel>()
            {
                MirrorBase =  new InArgument<string>(new CSharpValue<string>(mirrorBase)),
                TenantId = new InArgument<string>(new CSharpValue<string>(tenantId)),
                ModelKey = modelKey,
                Filter = new InArgument<string>(new CSharpValue<string>(filter)),
                Sort =  new InArgument<string>(new CSharpValue<string>(sort)),
                Result = new OutArgument<object>(new CSharpReference<object>(result))
            };
        }
    }
}