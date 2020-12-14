using System.Web.Http;
using System;
using System.Linq;
using WorkflowHost.Database;
using System.Collections.Generic;
using System.Text.Json;

namespace WorkflowHost.Controllers
{
    [RoutePrefix("workflow/definition")]
    public class WorkflowDesignController : ApiController
    {
      
        [Route("define"), HttpPost]
        public Guid Define([FromBody]WorkflowDefinition args)
        {
            using (var ctx = new WorkflowDefinitionContext())
            {
                var id = Guid.NewGuid();
                args.Id = id;
                args.CreatedAt = DateTime.Now;
                ctx.WorkflowDefinitions.Add(args);
                ctx.SaveChanges();  
                return id;
            }
        }

        [Route("list"), HttpGet]
        public IList<WorkflowDefinition> List()
        {
            using (var ctx = new WorkflowDefinitionContext())
            {
                return ctx.WorkflowDefinitions.ToList();
            }
        }

        [Route("remove"), HttpDelete]
        public void Remove(Guid id)
        {
            JsonSerializer.Deserialize<JsonElement>("{\"a\":1,\"b\":\"dsfdfdf\"}", null);
            using (var ctx = new WorkflowDefinitionContext())
            {
                var definition = ctx.WorkflowDefinitions.FirstOrDefault(d => d.Id == id);
                if (definition == null) return;
                ctx.WorkflowDefinitions.Remove(definition);
                ctx.SaveChanges();
            }
        }

        [Route("modify"), HttpPost]
        public void Modify([FromBody] WorkflowDefinition args)
        {
            using (var ctx = new WorkflowDefinitionContext())
            {
                var definition = ctx.WorkflowDefinitions.First(d => d.Id == args.Id);
                definition.Name = args.Name;
                definition.DisplayName = args.DisplayName;
                definition.Description = args.Description;
                definition.Definition = args.Definition;
                definition.Version = args.Version;
                ctx.SaveChanges();
              
            }
        }
    }
}