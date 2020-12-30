using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using System.Text;
using System.Threading.Tasks;

namespace WorkflowCore
{
    public class ModelGenerator
    {
        public Assembly Load()
        {
            string codeToCompile = @"
            using System;
namespace MyModel
{
    public class Xxx
    {
        public string Yyy { get; set; }
    }
}
";

          
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            string assemblyName = "MyModel";
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    var sb = new StringBuilder();
                    sb.AppendLine("runtime compile error:");
                    foreach (Diagnostic diagnostic in failures)
                    {
                        sb.AppendFormat("\t{0}: {1}\n", diagnostic.Id, diagnostic.GetMessage());
                    }
                    throw new Exception(sb.ToString());
                }
                else
                {
                 
                    ms.Seek(0, SeekOrigin.Begin);
                   return  AppDomain.CurrentDomain.Load(ms.ToArray());
                  
                }
            }

        }
    }
}
