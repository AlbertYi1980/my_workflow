using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkflowHost.Database
{
    public class WorkflowDefinition
    {        
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}