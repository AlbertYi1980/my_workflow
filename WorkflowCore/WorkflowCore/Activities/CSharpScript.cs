using System.Activities;
using WorkflowScripting;

namespace WorkflowCore.Activities
{
    public class CSharpScript : NativeActivity<string>
    {
        public string Code { get; set; }

        public InArgument<string> Args { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
           var result =   new CSharpExecuter(Code)
                .Use("System").Use("Newtonsoft.Json").Use("Newtonsoft.Json.Linq")
                .Execute<string, string>(Args.Get(context));
            Result.Set(context, result);
        }
    }

}
