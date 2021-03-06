﻿using System.Activities;

namespace Mirror.Workflows.Activities.Specials
{
    public  class UserTask: NativeActivity<string>
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
            Result.Set(context, (string)value);
            System.Console.WriteLine("resume");
        }
    }

   
}