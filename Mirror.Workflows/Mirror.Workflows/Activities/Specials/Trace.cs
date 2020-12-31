using System.Activities;
using System.Activities.Statements;
using DTrace = System.Diagnostics.Trace;

namespace Mirror.Workflows.Activities.Specials
{
    public  class Trace: NativeActivity<string>
    {
        public InArgument<string> Text { get; set; }
        
        protected override void Execute(NativeActivityContext context)
        {
           
            DTrace.WriteLine(Text.Get(context));
            DTrace.AutoFlush = true;
            DTrace.Flush();
            
            
        }

    }
}