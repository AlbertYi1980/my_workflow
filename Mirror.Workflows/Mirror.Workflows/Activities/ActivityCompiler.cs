using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UiPath.Workflow;

namespace Mirror.Workflows.Activities
{
    public class ActivityCompiler
    {
        private readonly IEnumerable<Assembly> _refAssemblies;
        private readonly IEnumerable<string> _usingNamespaces;
        
        
        public ActivityCompiler(IEnumerable<Assembly> refAssemblies, IEnumerable<string> usingNamespaces)
        {
            _refAssemblies = refAssemblies;
            _usingNamespaces = usingNamespaces;
        }

        public void Compile(string activityNamespace, string activityName, Activity activity)
        {
            if (_refAssemblies != null)
            {
                TextExpression.SetReferences(activity, _refAssemblies.Select(a => new AssemblyReference {Assembly = a}).ToArray());
                
            }

            if (_usingNamespaces != null)
            {
                  TextExpression.SetNamespaces(activity, _usingNamespaces.ToArray());
            }
            var settings = new TextExpressionCompilerSettings
            {
                Activity = activity,
                Language = "C#",
                ActivityName = activityName,
                ActivityNamespace = activityNamespace,
                RootNamespace = null,
                GenerateAsPartialClass = false,
                AlwaysGenerateSource = true,
                ForImplementation = false,
                Compiler = new CSharpAheadOfTimeCompiler(),
            };
            var results = new TextExpressionCompiler(settings).Compile();
            if (results.HasErrors)
            {
                throw new Exception("Compilation failed.");
            }
            var compiledExpressionRoot = Activator.CreateInstance(results.ResultType, activity) as ICompiledExpressionRoot;
            CompiledExpressionInvoker.SetCompiledExpressionRoot(activity, compiledExpressionRoot);
        }
    }
}