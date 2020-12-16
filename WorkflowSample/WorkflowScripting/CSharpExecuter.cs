using CSScriptLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowScripting
{
    public class CSharpExecuter
    {
        private readonly List<string> _usingList = new List<string>();
        private readonly string body;
        public CSharpExecuter(string code)
        {
            body = code;
        }

        public CSharpExecuter Use(string @namespace)
        {
            _usingList.Add(@namespace);
            return this;
        }

        public MethodDelegate<TOut> GetFunc<TIn, TOut>(string argsName = "args")
        {
            var sb = new StringBuilder();
            foreach (var @namespace in _usingList)
            {
                sb.AppendLine($"using {@namespace};");
            }
            var inType = typeof(TIn).Name;
            var outType = typeof(TOut).Name;
            sb.AppendLine($"{outType} func({inType} {argsName})");
            sb.AppendLine("{");
            sb.Append(body);
            sb.AppendLine("}");
            return CSScript.CreateFunc<TOut>(sb.ToString());
        }

        public TOut Execute<TIn, TOut>(TIn args)
        {
            var _func = GetFunc<TIn, TOut>();
            return _func(args);
        }

        public object Execute(object args)
        {
            return Execute<object, object>(args);
        }
    }
}
