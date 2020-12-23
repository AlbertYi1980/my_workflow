using Natasha.CSharp;
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
        static CSharpExecuter()
        {
            NatashaInitializer.InitializeAndPreheating(true);
        }
        public CSharpExecuter(string code)
        {
            body = code;
        }

        public CSharpExecuter Use(string @namespace)
        {
            _usingList.Add(@namespace);
            return this;
        }

        public Func<TIn, TOut> GetFunc<TIn, TOut>(string argsName = "args")
        {
            return FastMethodOperator.RandomDomain(o =>
                            {
                                o.ThrowAndLogCompilerError();
                                o.ThrowAndLogSyntaxError();
                                o.UseStreamCompile();
                                foreach (var ns in _usingList)
                                {
                                    o.Add($"using {ns};");
                                }
                            }).Param<TIn>(argsName)
                            .Body(body)
                            .Return<TOut>()
                            .Compile<Func<TIn, TOut>>();
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
