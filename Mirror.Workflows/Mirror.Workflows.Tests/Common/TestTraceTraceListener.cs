using System.Diagnostics;
using System.Threading;
using Xunit.Abstractions;

namespace Mirror.Workflows.Tests.Common
{
    internal class TestTraceTraceListener : TraceListener
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestTraceTraceListener(ITestOutputHelper outputHelper)
        {
        
            _outputHelper = outputHelper;
        }
        public override void Write(string message)
        {
            _outputHelper.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            var foo = Thread.CurrentThread.ManagedThreadId;
            _outputHelper.WriteLine(message);
        }
    }
}