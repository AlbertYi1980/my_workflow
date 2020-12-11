using System.Web.Http;
using System;

namespace WorkflowHost.Controllers
{
    [RoutePrefix("workflow")]
    public class WorkflowDesignController : ApiController
    {
        public class WorkflowDefinition
        {
            public string Name { get; set; }
            public string Definition { get; set; }
            public string Version { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
        }

        [Route("define"), HttpPost]
        public Guid Define([FromBody]WorkflowDefinition args)
        {
            var id = Guid.NewGuid();
            // generate xaml
            return id;
        }
    }
}