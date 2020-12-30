using System.Activities;

namespace Mirror.Workflows.Activities.Special
{
    public  class Trace: NativeActivity<string>
    {
        public InArgument<string> Text { get; set; }
        
        protected override void Execute(NativeActivityContext context)
        {
            System.Diagnostics.Trace.WriteLine(Text.Get(context));
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Flush();
        }

    }
}