using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Text.Json;
using System.Activities.XamlIntegration;
using System.IO;
using WorkflowCore;
using WorkflowHost.Database;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WorkflowCore.Stores;

namespace WorkflowHost.Controllers
{
    [Route("workflow")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IMongoCollection<WorkflowDefinition> _defCollection;
        private readonly IOptions<MongoOptions> _options;
        public WorkflowController(IOptions<MongoOptions> options)
        {
            var mongoOptions = options.Value;
            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.Database);
            _defCollection = database.GetCollection<WorkflowDefinition>("workflow_definition");
        }

        public class CreateArgs
        {

            public string FlowName { get; set; }

        }

        [Route("create"), HttpPost]
        public Guid Create([FromBody] CreateArgs args)
        {
            var app = new WorkflowApplication(BuildActivity(args.FlowName));
            var store = new MongodbInstanceStore(_options);
            app.InstanceStore = store;
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Run();

            return app.Id;
        }

        public class CompleteUserTaskArgs
        {
            public Guid Id { get; set; }
            public string FlowName { get; set; }
            public string TaskName { get; set; }
            public string Args { get; set; }
        }

        [Route("complete-user-task"), HttpPost]
        public void CompleteUserTask([FromBody] CompleteUserTaskArgs args)
        {
            var app = new WorkflowApplication(BuildActivity(args.FlowName));
            var store = new MongodbInstanceStore(_options);
            app.InstanceStore = store;
            app.PersistableIdle = eventArgs => PersistableIdleAction.Unload;
            app.Load(args.Id);
            var result = app.ResumeBookmark(args.TaskName, JsonSerializer.Deserialize<JsonElement>(args.Args));
        }

        private Activity BuildActivity(string name)
        {

            var convertor = new Json2Xaml();
            var json = LoadJson(name);
            var xaml = convertor.Convert(name, json);
            var settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true,


            };
            var activity = ActivityXamlServices.Load(new StringReader(xaml), settings);
            return activity;
        }

        private string GetConnectionString()
        {
            return "Data Source=tcp:qds112528109.my3w.com;Initial Catalog=qds112528109_db;Persist Security Info=True;User ID=qds112528109;Password=AAaa1234567";
        }

        private string LoadJson(string name)
        {
            return _defCollection.Find(i => i.Name == name).First().Definition;
        }
    }
}