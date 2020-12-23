using CustomInstanceStore;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Threading;
using System.Threading.Tasks;

namespace MyWorkflow
{
    class Program
    {

        private static Activity1 _activity = new Activity1();    
        private static AutoResetEvent sync = new AutoResetEvent(false);
        static void Main(string[] args)
        {

            //var appId = StartWorkflow();

            var appId = new Guid("2A0E1FB7-7C5E-45A9-BCBC-AD8EB8DD3198");
            Resume(appId, "c");
            //var marks = GetBookemarks(appId);

            //foreach(var mark in marks)
            //{
            //    Console.WriteLine(mark);
            //}

            sync.WaitOne();

            Console.WriteLine("enter to end..");
            Console.ReadLine();

            
        }

        private static Guid StartWorkflow()
        {
             var app = new WorkflowApplication(_activity);
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Unloaded += _ => sync.Set();
            app.InstanceStore = GetStore();
            app.Run();
            Console.WriteLine(app.Id);
            return app.Id;
        }

        private static void Resume(Guid appId, string name)
        {
            var app = new WorkflowApplication(_activity);
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Unloaded += _ => sync.Set();
        
            app.InstanceStore = GetStore();    
            app.Load(appId);
            var reuslt =   app.ResumeBookmark(name, $"data:{name}");
            if (reuslt == BookmarkResumptionResult.NotFound)
            {
                app.Unload();
            }
        }

        private static IList<string> GetBookemarks(Guid appId)
        {
            var app = new WorkflowApplication(_activity);
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Unloaded += _ => sync.Set();
            app.InstanceStore = GetStore();
            app.Load(appId);
            var marks = app.GetBookmarks().Select(b => b.BookmarkName).ToList();
            app.Unload();
            return marks;
        }

        private static Activity CreateCustomActivity()
        {
            var stateMachine = new StateMachine()
            {

            };
           
            return stateMachine;
        }

        private static string GetConnectionString()
        {
            return "Data Source=tcp:qds112528109.my3w.com;Initial Catalog=qds112528109_db;Persist Security Info=True;User ID=qds112528109;Password=AAaa1234567";
        }

        
        private static InstanceStore GetStore()
        {
            return new SqlWorkflowInstanceStore(GetConnectionString());
            //return new MemoryCacheInstanceStore(new Guid("6F8B45EB-C9C8-4AEB-9E07-5D52280EA601"));
        }

        
    }
}
