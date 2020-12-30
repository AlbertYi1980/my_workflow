using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Mirror.Workflows.Activities;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.Tests.Common;
using Mirror.Workflows.TypeManagement;
using Xunit;
using Xunit.Abstractions;

namespace Mirror.Workflows.Tests
{
    public class ActivityExecutingTest
    {
        public ActivityExecutingTest(ITestOutputHelper outputHelper)
        {
            System.Diagnostics.Trace.Listeners.Add(new TestTraceTraceListener(outputHelper));
        }
        
        [Fact]
        public void RunIf()
        {
            var parser = new CompositeActivityParser(new WellKnownDescriptorContainer(), new TypeContainer());
           var definition = LoadDefinition("if");
            var activity = parser.Parse(definition);
            var compiler = new ActivityCompiler(null, null);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null, null, new TypesInfoProvider(null, null, null));
            executor.Run( activity);
        }
        
        [Fact]
        public void RunTrace()
        {
         
            var parser = new CompositeActivityParser(new WellKnownDescriptorContainer(), new TypeContainer());
            var definition = LoadDefinition("trace");
            var activity = parser.Parse(definition);
            var compiler = new ActivityCompiler(null, null);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null, null, new TypesInfoProvider(null, null, null));
            executor.Run( activity);

         
        }
        
        [Fact]
        public void RunDynamic()
        {
            var code = @"
    using System;
namespace MyModel
{
    public class Xxx
    {
        public string Yyy { get; set; }
    }
}
";
            var assemblyName = "t_123_v_1.1";
            
            var parser = new CompositeActivityParser(new WellKnownDescriptorContainer(), new TypeContainer());
            var definition = LoadDefinition("dynamic");
            var activity = parser.Parse(definition);
            var compiler = new ActivityCompiler(null, null);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null, null, new TypesInfoProvider(null, assemblyName, code));
            executor.Run( activity);
            
            executor.Run(activity);

        }

   

        private string LoadDefinition(string name)
        {
            var dir = @"C:\Users\yicheng\source\repos\my_workflow\Mirror.Workflows\Mirror.Workflows.Tests\cases";
            var path = Path.Combine(dir, $"{name}.json");
            return File.ReadAllText(path);
        }
    }
}