using Xunit;

namespace Mirror.Workflows.Tests
{
    public class TypeInfoProviderTest
    {
        [Fact]
        public void GetStaticTypesInfo()
        {
           
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


           
        }


        [Fact]
        public void FriendlyNameTest()
        {
      
        }
    }
    

    
}