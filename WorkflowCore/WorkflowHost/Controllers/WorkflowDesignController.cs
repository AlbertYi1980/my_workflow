using System;
using System.Linq;
using WorkflowCore;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WorkflowHost.Database;

namespace WorkflowHost.Controllers
{
    [Route("workflow/definition")]
    [ApiController]
    public class WorkflowDesignController : ControllerBase
    {
        private readonly IMongoCollection<WorkflowDefinition> _defCollection;
        public WorkflowDesignController(IOptions<MongoOptions> options)
        {
            var mongoOptions = options.Value;
            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.Database);
            _defCollection = database.GetCollection<WorkflowDefinition>("workflow_definition");
        }

        [Route("define"), HttpPost]
        public string Define([FromBody] WorkflowDefinition args)
        {
            args.CreatedAt = DateTime.Now;
            _defCollection.InsertOne(args);
            return args.Id;
        }

        [Route("list"), HttpGet]
        public IList<WorkflowDefinition> List()
        {
            return _defCollection.Find(i => true).ToList();
        }

        [Route("remove"), HttpDelete]
        public void Remove(string id)
        {
            JsonSerializer.Deserialize<JsonElement>("{\"a\":1,\"b\":\"dsfdfdf\"}", null);
            _defCollection.DeleteOne(i => i.Id == id);
        }

        [Route("modify"), HttpPost]
        public void Modify([FromBody] WorkflowDefinition args)
        {
            var updateBuilder = Builders<WorkflowDefinition>.Update
                .Set(i => i.Name, args.Name)
                .Set(i => i.DisplayName, args.DisplayName)
                .Set(i => i.Definition, args.Definition)
                .Set(i => i.Description, args.Description)
                .Set(i => i.Version, args.Version);
            _defCollection.UpdateOne(i => i.Id == args.Id, updateBuilder);
        }
    }
}