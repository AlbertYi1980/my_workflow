using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WorkflowCore.Activities
{
    // 主对象存在于mirror中
    //拥有一个state状态

    public class StandardStateMachine : NativeActivity
    {
        [DefaultValue(null)]
        public Activity StateMachine { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            Console.WriteLine("ok");
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            if (StateMachine == null) return;

            Collection<Activity> collection = new Collection<Activity>()
            {
                this.StateMachine
            };


            metadata.SetChildrenCollection(collection);
        }

       
    }
}