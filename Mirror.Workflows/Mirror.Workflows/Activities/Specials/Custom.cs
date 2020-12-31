using System;
using System.Activities;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Mirror.Workflows.Activities.Specials
{
    public sealed class Custom : NativeActivity
    {
        private Activity Body { get; set; }
       

   
        public Custom()
        {
          
        }

 
        protected override void Execute(NativeActivityContext context)
        {
        
            context.ScheduleActivity(Body);
        }

       
    }
}