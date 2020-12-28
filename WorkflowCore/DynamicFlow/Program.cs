using Microsoft.CSharp.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Linq;
using System.Reflection;
using UiPath.Workflow;

namespace DynamicFlow
{
    class Program
    {
        private static Assembly _assembly;
        static void Main(string[] args)
        {
            _assembly = new ModelLoader().Load();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            var activity = new ActivityGenerator().Generate();

            CompileExpressions(activity);

            WorkflowInvoker.Invoke(activity);


        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("MyModel"))
            {
                return _assembly;
            }
            return null;
        }


        static void CompileExpressions(Activity activity)
        {
            // activityName is the Namespace.Type of the activity that contains the
            // C# expressions.
            string activityName = activity.GetType().ToString();

            // Split activityName into Namespace and Type.Append _CompiledExpressionRoot to the type name
            // to represent the new type that represents the compiled expressions.
            // Take everything after the last . for the type name.
            string activityType = activityName.Split('.').Last() + "_CompiledExpressionRoot";
            // Take everything before the last . for the namespace.
            string activityNamespace = string.Join(".", activityName.Split('.').Reverse().Skip(1).Reverse());

            var assembly = _assembly;
            var assemblyReference = new AssemblyReference();
            assemblyReference.Assembly = assembly;

            TextExpression.SetReferences(activity, new[] { assemblyReference });
            TextExpression.SetNamespaces(activity, new[] { "MyModel" });

            // Create a TextExpressionCompilerSettings.
            TextExpressionCompilerSettings settings = new TextExpressionCompilerSettings
            {
                Activity = activity,
                Language = "C#",
                ActivityName = activityType,
                ActivityNamespace = activityNamespace,
                RootNamespace = null,
                GenerateAsPartialClass = false,
                AlwaysGenerateSource = true,
                ForImplementation = false,
                Compiler = new CSharpAheadOfTimeCompiler(),
                LogSourceGenerationMessage = (info) => Console.WriteLine(info),
            };

            // Compile the C# expression.
            TextExpressionCompilerResults results = new TextExpressionCompiler(settings).Compile();

            // Any compilation errors are contained in the CompilerMessages.
            if (results.HasErrors)
            {
                throw new Exception("Compilation failed.");
            }

            // Create an instance of the new compiled expression type.
            ICompiledExpressionRoot compiledExpressionRoot =
                Activator.CreateInstance(results.ResultType,
                    new object[] { activity }) as ICompiledExpressionRoot;

            // Attach it to the activity.
            CompiledExpressionInvoker.SetCompiledExpressionRoot(
                activity, compiledExpressionRoot);
        }
    }
}
