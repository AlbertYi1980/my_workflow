using System;
using Mirror.Workflows.TypeManagement;
using Xunit;

namespace Mirror.Workflows.Tests
{
    public class DynamicLoadingTest
    {
        [Fact]
        public void LoadCode()
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
            var assembly =   DynamicModelLoader.Load(assemblyName, code);
            var type  = Type.GetType($"MyModel.Xxx,{assemblyName}");
            Assert.NotNull(type);
        }
        
        [Fact]
        public void LoadCodeTwice()
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
            var assembly =   DynamicModelLoader.Load(assemblyName, code);
             DynamicModelLoader.Load(assemblyName, code);
            var type  = Type.GetType($"MyModel.Xxx,{assemblyName}");
            Assert.NotNull(type);
        }
    }
}