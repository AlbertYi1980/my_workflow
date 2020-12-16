using CustomInstanceStore;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Statements;
using System.Dynamic;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Threading.Tasks;

namespace MyWorkflow
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var activity = new Activity1();
            var app = new WorkflowApplication(activity);



            app.InstanceStore = GetStore();
            app.PersistableIdle = eventArgs =>
            {
                return PersistableIdleAction.Unload;
            };
            app.Run();
            Console.WriteLine(app.Id);
            Console.WriteLine(DateTime.Now);
            await Task.Delay(TimeSpan.FromSeconds(4));
            Console.WriteLine(DateTime.Now);


            //var app2 = new WorkflowApplication(activity);
            //app2.InstanceStore = GetStore();
            //app2.Load(app.Id);
            //app2.ResumeBookmark("input", "{\"data\":{\"name\":\"aaaa\",\"age\":3231}}");
            Console.WriteLine("enter to end..");
            Console.ReadLine();

            
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
