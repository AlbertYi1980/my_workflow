using System;
using System.Activities;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities
{
    public class ActivityExecutionContext
    {
        public ResourceScope ResourceScope { get; set; }
        public IDescriptorProvider DescriptorProvider { get; set; }
        public TypeContainer TypeContainer { get; set; }
        public InstanceStore InstanceStore { get; set; }
    }
    
    public class ActivityExecutor
    {
        private readonly IEnumerable<IActivityDescriptor> _descriptors;
        private readonly InstanceStore _store;
   
        private readonly ActivityCompiler _compiler;
        private readonly TypesInfo _info;

        public ActivityExecutor(IEnumerable<IActivityDescriptor> descriptors, InstanceStore store, ITypesProvider typesProvider)
        {
            _descriptors = descriptors;
            _store = store;
            _info = typesProvider?.Get();
            _compiler = new ActivityCompiler(_info?.RefAssemblies, _info?.UsingNamespaces);
        }
        
        public Guid Run( Activity activity)
        {
            var unloadedSignal = new AutoResetEvent(false);
            // var parser = new CompositeActivityParser(new DescriptorProvider(), new TypeContainer());
            // var activity = parser.Parse(definition);
            // _compiler.Compile(activityNamespace, activityName, activity);
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

        public bool Resume(Activity activity, Guid appId, string bookmarkName, object bookmarkArgs)
        {          
            var unloadedSignal = new AutoResetEvent(false);
            // var parser = new CompositeActivityParser(_descriptors, _info.Types);
            // var activity = parser.Parse(definition);
            // _compiler.Compile(activityNamespace, activityName, activity);
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