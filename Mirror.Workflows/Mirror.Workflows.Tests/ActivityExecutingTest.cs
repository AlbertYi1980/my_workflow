using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Mirror.Workflows.Activities;
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
            var executor = new ActivityExecutor(null, null, new TypesInfoProvider(null, null, null));

            var definition = LoadDefinition("if");
            executor.Run("nnn", "mmmm", definition);
        }
        
        [Fact]
        public void RunTrace()
        {
         
            var executor = new ActivityExecutor(null, null, new TypesInfoProvider(null, null, null));

            var definition = LoadDefinition("trace");
            executor.Run("nnn", "mmmm", definition);

         
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
         
            var executor = new ActivityExecutor(null, null, new TypesInfoProvider(null, assemblyName, code));

            var definition = LoadDefinition("dynamic");
            executor.Run("nnn", "mmmm", definition);

        }

   

        private string LoadDefinition(string name)
        {
            var dir = @"C:\Users\yicheng\source\repos\my_workflow\Mirror.Workflows\Mirror.Workflows.Tests\cases";
            var path = Path.Combine(dir, $"{name}.json");
            return File.ReadAllText(path);
        }
    }
}