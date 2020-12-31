using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities;
using Mirror.Workflows.Activities.Specials;
using Mirror.Workflows.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Mirror.Workflows.Tests
{
    public class MsWorkflowTest
    {
        public MsWorkflowTest(ITestOutputHelper outputHelper)
        {
            System.Diagnostics.Trace.Listeners.Add(new TestTraceTraceListener(outputHelper));
        }
        
        [Fact]
        public void RunDynamicActivity()
        {
   
            Activity dynamicWorkflow = new DynamicActivity()  
            {  
              
                Implementation = () => new Sequence()  
                {  
                    Activities =
                    {  
                        new Trace()  
                        {  
                            Text = new InArgument<string>(new CSharpValue<string>("\"hfffello\""))  
                        }  
                    }  
                }  
            };  

            var executor = new ActivityExecutor(null);
            executor.Run( dynamicWorkflow);
   
            // var app = new WorkflowApplication(dynamicWorkflow);
            // app.Run();
            //
  
        }
    }
}