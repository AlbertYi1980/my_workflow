using ConsoleHost;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Web.Http;
using System.Text.Json;
using System.Activities.XamlIntegration;
using System.IO;
using WorkflowCore;

namespace WorkflowHost.Controllers
{
    [RoutePrefix("workflow")]
    public class WorkflowController : ApiController
    {
        [Route("create"), HttpPost]
        public Guid Create()
        {
            var app = new WorkflowApplication(BuildActivity());
            var store = new SqlWorkflowInstanceStore(
                "Data Source=tcp:qds112528109.my3w.com;Initial Catalog=qds112528109_db;Persist Security Info=True;User ID=qds112528109;Password=AAaa1234567");
            app.InstanceStore = store;
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Run();

            return app.Id;
        }

        public class CompleteUserTaskArgs
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Args { get; set; }
        }

        [Route("complete-user-task"), HttpPost]
        public void CompleteUserTask([FromBody]CompleteUserTaskArgs args)
        {
            var app = new WorkflowApplication(BuildActivity());
            var store = new SqlWorkflowInstanceStore(
                "Data Source=tcp:qds112528109.my3w.com;Initial Catalog=qds112528109_db;Persist Security Info=True;User ID=qds112528109;Password=AAaa1234567");
            app.InstanceStore = store;
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Load(args.Id);
            var result = app.ResumeBookmark(args.Name, JsonSerializer.Deserialize<JsonElement>( args.Args));
        }

        private Activity BuildActivity()
        {

            var convertor = new Json2Xaml();
            var json = "{\"$type\":\"sequence\",\"activities\":[{\"$type\":\"writeLine\",\"text\":\"\\\"before action.\\\"\"},{\"$type\":\"userTask\",\"name\":\"aaa\"},{\"$type\":\"writeLine\",\"text\":\"\\\"after action.\\\"\"}]}";
            var xaml = convertor.Convert("aaFirst", json);
            var settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true,


            };
            var activity = ActivityXamlServices.Load(new StringReader(xaml), settings);
            return activity;
        }
    }
}