using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;

namespace WorkflowHost.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public string Welcome()
        {
            return "hello, workflow.";
        }
    }
}