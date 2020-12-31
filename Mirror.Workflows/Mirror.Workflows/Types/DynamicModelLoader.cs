using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mirror.Workflows.Types
{
    public static class DynamicModelLoader
    {
        private static readonly ConcurrentDictionary<string, Assembly> Assemblies = new ConcurrentDictionary<string, Assembly>();

        static  DynamicModelLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private static  Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(",")[0];
            if (!Assemblies.TryGetValue(assemblyName, out var assembly))
            {
                return null;
            }
            return assembly;
        }

        public static Assembly Load(string assemblyName, string code)
        {
            var codeToCompile = code;
            var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            var directoryName = Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location);
            var refPaths = new[] {
                typeof(object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(directoryName!, "System.Runtime.dll")
            };
            var references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {

                var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                var sb = new StringBuilder();
                sb.AppendLine("runtime compile error:");
                foreach (var diagnostic in failures)
                {
                    sb.AppendFormat("\t{0}: {1}\n", diagnostic.Id, diagnostic.GetMessage());
                }
                throw new Exception(sb.ToString());
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = AppDomain.CurrentDomain.Load(ms.ToArray());
            Assemblies.TryAdd(assemblyName, assembly);
            return assembly;
        }
    }
}