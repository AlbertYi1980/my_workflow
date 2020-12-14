using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorkflowCore.Activities
{
    public  class UserTask: NativeActivity<JsonElement>
    {
        public string Name { get; set; }

        protected override bool CanInduceIdle => true;

        protected override void Execute(NativeActivityContext context)
        {
             context.CreateBookmark(Name, Callback);

            
           
        }

        private void Callback(NativeActivityContext context, Bookmark bookmark, object value)
        {
            var args = value is JsonElement element ? element : new JsonElement();
            context.RemoveBookmark(Name);
            Result .Set(context,args );
   
            
        }
    }
}
