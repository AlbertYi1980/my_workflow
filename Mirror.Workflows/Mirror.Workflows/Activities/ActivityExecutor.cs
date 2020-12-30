using System;
using System.Activities;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
using System.Threading;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities
{
    public class ActivityExecutor
    {
        private readonly IEnumerable<ActivityDescriptor> _descriptors;
        private readonly InstanceStore _store;
   
        private readonly ActivityCompiler _compiler;
        private readonly TypesInfo _info;

        public ActivityExecutor(IEnumerable<ActivityDescriptor> descriptors, InstanceStore store, ITypesProvider typesProvider)
        {
            _descriptors = descriptors;
            _store = store;
            _info = typesProvider?.Get();
            _compiler = new ActivityCompiler(_info?.RefAssemblies, _info?.UsingNamespaces);
        }
        
        public Guid Run( string activityNamespace, string activityName,string definition)
        {
            var unloadedSignal = new AutoResetEvent(false);
            var parser = new ActivityParser(_descriptors, _info.Types);
            var activity = parser.Parse(definition);
            _compiler.Compile(activityNamespace, activityName, activity);
            var app = new WorkflowApplication(activity)
            {
                InstanceStore = _store,
                PersistableIdle = eventArgs => PersistableIdleAction.Unload,
                Unloaded = args => unloadedSignal.Set()
            };
            app.Run();
            unloadedSignal.WaitOne();
            return app.Id;
        }

        public bool Resume(string activityNamespace, string activityName,string definition, Guid appId, string bookmarkName, object bookmarkArgs)
        {          
            var unloadedSignal = new AutoResetEvent(false);
            var parser = new ActivityParser(_descriptors, _info.Types);
            var activity = parser.Parse(definition);
            _compiler.Compile(activityNamespace, activityName, activity);
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