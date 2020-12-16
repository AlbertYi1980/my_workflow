using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.Net.Http;
using System.Text;


namespace WorkflowCore.Activities
{
    public abstract class MirrorCrud<T> : NativeActivity<T>
    {
        public InArgument<string> MirrorBase { get; set; }
        public InArgument<string>   TenantId { get; set; }
        public InArgument<string> ModelKey { get; set; }
  
    }
}