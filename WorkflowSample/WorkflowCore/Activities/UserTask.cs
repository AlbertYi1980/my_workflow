using Newtonsoft.Json.Linq;
using System.Activities;


namespace WorkflowCore.Activities
{
    public  class UserTask: NativeActivity<JToken>
    {
        public string Name { get; set; }

        protected override bool CanInduceIdle => true;

        protected override void Execute(NativeActivityContext context)
        {
             context.CreateBookmark(Name, Callback);

            
           
        }

        private void Callback(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.RemoveBookmark(Name);
            Result .Set(context,(JToken) value );
   
            
        }
    }
}
