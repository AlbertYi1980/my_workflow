using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowCore
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; } = "cube_data";
    }
}
