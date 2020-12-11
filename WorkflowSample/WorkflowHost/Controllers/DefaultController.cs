using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WorkflowHost.Controllers
{
    public class DefaultController : ApiController
    {
        [Route, HttpGet]
        public string Welcome()
        {
            return "hello, workflow.";
        }
    }
}