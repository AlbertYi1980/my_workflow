using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Mirror.Workflows.Tests
{
    public class SerializationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SerializationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Foo()
        {
            
            var serializer = new DataContractJsonSerializer(typeof(A));
            var a = new A( "k" );
            var buff = new MemoryStream();
     
            serializer.WriteObject(buff, a);
         

            var s = Encoding.UTF8.GetString(buff.GetBuffer());
            buff.Position = 0;
            var result = serializer.ReadObject(buff);

            _testOutputHelper.WriteLine(s);
        }
    }
    
    [DataContract]
    public class A
    {
        public A( string c )
        {
            B = new B<string>() { C = c};
        }
        [DataMember]
        public B<string> B { get; set; }
    }

    [DataContract]
    public class B<T>
    {
        [DataMember]
        public T C { get; set; }
    }
}