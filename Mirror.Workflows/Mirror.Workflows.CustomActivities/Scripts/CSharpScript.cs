using System.Activities;
using WorkflowScripting;

namespace Mirror.Workflows.CustomActivities.Scripts
{
    public class CSharpScript<TInput, TOutput> : NativeActivity<TOutput>
    {
        public string Code { get; set; }

        public InArgument<TInput> Arguments { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var result =   new CSharpExecutor(Code)
                .Use("System")
                .Execute<TInput, TOutput>(Arguments.Get(context));
            Result.Set(context, result);
        }
    }
    
  

}