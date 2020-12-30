using System.Activities;
using DTrace = System.Diagnostics.Trace;

namespace Mirror.Workflows.Activities.Special
{
    public  class Trace: NativeActivity<string>
    {
        public InArgument<string> Text { get; set; }
        
        protected override void Execute(NativeActivityContext context)
        {
           
            DTrace.WriteLine(Text.Get(context));
            DTrace.Flush();
        }

    }
}