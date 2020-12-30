using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mirror.Workflows.TypeManagement;
using Xunit;

namespace Mirror.Workflows.Tests
{
    public class TypeInfoProviderTest
    {
        [Fact]
        public void GetStaticTypesInfo()
        {
            var assemblies = new List<Assembly> {typeof(A).Assembly};
            var namespaces = new[] {"Mirror.Workflows.Tests"};
            var provider = new TypesInfoProvider(new TypesInfo(assemblies, namespaces, new Type[]{}), null, null);
            var info = provider.Get();
            var assemblyName = info.RefAssemblies.FirstOrDefault();
        }

        [Fact]
        public void GetDynamicTypesInfo()
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


            var provider = new TypesInfoProvider(null, assemblyName, code);
            var info = provider.Get();
        }
    }

    public class A
    {
    }
}