using System;
using System.Activities;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
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

        public Guid Run(Activity activity, IDictionary<string, object> args = null)
        {
            var unloadedSignal = new AutoResetEvent(false);

            var app = new WorkflowApplication(activity, args ?? new Dictionary<string, object>())
            {
                InstanceStore = _store,
                PersistableIdle = eventArgs => PersistableIdleAction.Unload,
                Aborted = obj =>
                {
                    unloadedSignal.Set();
                    Trace.TraceError("abort");
                },
                OnUnhandledException = (a) =>
                {
                    Trace.TraceError($"exception:{a.UnhandledException.Message}");
                    return UnhandledExceptionAction.Cancel;
                },
                Unloaded = a => unloadedSignal.Set(),
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
                Aborted = obj =>
                {
                    unloadedSignal.Set();
                    Trace.TraceError("abort");
                },
                OnUnhandledException = (a) =>
                {
                    Trace.TraceError($"exception:{a.UnhandledException.Message}");
                    return UnhandledExceptionAction.Cancel;
                },
                Unloaded = _ =>
                {
                    unloadedSignal.Set();
                    Trace.TraceError("unloaded");
                },
                Completed = _ =>
                {
                    unloadedSignal.Set();
                    Trace.TraceError("completed");
                }
            };
            app.Load(appId);
            var result = app.ResumeBookmark(bookmarkName, bookmarkArgs);
            if (result != BookmarkResumptionResult.Success) return false;
            unloadedSignal.WaitOne();
            return true;
        }
    }
}