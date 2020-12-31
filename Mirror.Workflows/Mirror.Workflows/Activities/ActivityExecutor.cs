using System;
using System.Activities;
using System.Activities.Runtime.DurableInstancing;
using System.Diagnostics;
using System.Threading;
using Activity = System.Activities.Activity;

namespace Mirror.Workflows.Activities
{
    public class ActivityExecutor
    {
        private readonly InstanceStore _store;

        public ActivityExecutor(InstanceStore store)
        {
            _store = store;
        
     
        }
        
        public Guid Run( Activity activity)
        {
            var unloadedSignal = new AutoResetEvent(false);
       
            var app = new WorkflowApplication(activity)
            {
                InstanceStore = _store,
                PersistableIdle = eventArgs => PersistableIdleAction.Unload,
                Aborted= args =>
                {
                    unloadedSignal.Set();
                    Trace.TraceError("abort");
                },
                OnUnhandledException = (args) =>
                {
                    Trace.TraceError($"exception:{args.UnhandledException.Message}");
                    return UnhandledExceptionAction.Cancel;
                },
                Unloaded = args => unloadedSignal.Set(),
                
            };
            
            app.Run();
            unloadedSignal.WaitOne();
            return app.Id;
        }

        public bool Resume(Activity activity, Guid appId, string bookmarkName, object bookmarkArgs)
        {          
            var unloadedSignal = new AutoResetEvent(false);
      
            var app = new WorkflowApplication(activity)
            {
                InstanceStore = _store,
                PersistableIdle = eventArgs => PersistableIdleAction.Unload,
                Unloaded = args => unloadedSignal.Set()
            };
            app.Load(appId);
            var result = app.ResumeBookmark(bookmarkName, bookmarkArgs);
            unloadedSignal.WaitOne();
            return result == BookmarkResumptionResult.Success;
        }
    }
}