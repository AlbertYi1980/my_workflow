using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mirror.Workflows.Types;
using UiPath.Workflow;

namespace Mirror.Workflows.Activities
{
    public class ActivityCompiler
    {
        private readonly IEnumerable<Assembly> _refAssemblies;
        private readonly IEnumerable<string> _usingNamespaces;


        public ActivityCompiler(ITypeInfoProvider typeInfoProvider)
        {
            _refAssemblies = typeInfoProvider.ListAssemblies();
            _usingNamespaces = typeInfoProvider.ListNamespaces();
        }

        public void Compile(string activityNamespace, string activityName, Activity activity, bool forImplementation = true)
        {
            if (_refAssemblies != null)
            {
                var references = _refAssemblies.Select(a => new AssemblyReference {Assembly = a}).ToArray();
                if (!forImplementation)
                {
                    TextExpression.SetReferences(activity, references);
                }
                else
                {
                    TextExpression.SetReferencesForImplementation(activity, references);
                }
            }

            if (_usingNamespaces != null)
            {
                var namespaces = _usingNamespaces.ToArray();
                if (!forImplementation)
                {
                    TextExpression.SetNamespaces(activity, namespaces);
                }
                else
                {
                    TextExpression.SetNamespacesForImplementation(activity, namespaces);
                }
            }

            var settings = new TextExpressionCompilerSettings
            {
                Activity = activity,
                Language = "C#",
                ActivityName = activityName,
                ActivityNamespace = activityNamespace,
                RootNamespace = "dd",
                GenerateAsPartialClass = false,
                AlwaysGenerateSource = false,
                ForImplementation = forImplementation,
                Compiler = new CSharpAheadOfTimeCompiler(),
            };
            var results = new TextExpressionCompiler(settings).Compile();
            if (results.HasErrors)
            {
                throw new Exception("Compilation failed.");
            }
            

            if (results.ResultType == null) return;
            var compiledExpressionRoot =
                Activator.CreateInstance(results.ResultType, activity) as ICompiledExpressionRoot;

            if (!forImplementation)
            {
                CompiledExpressionInvoker.SetCompiledExpressionRoot(activity, compiledExpressionRoot);
            }
            else
            {
                CompiledExpressionInvoker.SetCompiledExpressionRootForImplementation(activity, compiledExpressionRoot);
            }
        }
    }
}