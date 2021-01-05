using Natasha.CSharp;

using System;
using System.Collections.Generic;


namespace WorkflowScripting
{
    public class CSharpExecutor
    {
        private readonly List<string> _usingList = new List<string>();
        private readonly string _body;
        static CSharpExecutor()
        {
            NatashaInitializer.InitializeAndPreheating(true);
        }
        public CSharpExecutor(string code)
        {
            _body = code;
        }

        public CSharpExecutor Use(string @namespace)
        {
            _usingList.Add(@namespace);
            return this;
        }

        private Func<TIn, TOut> GetFunc<TIn, TOut>(string argsName = "arguments")
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
                            .Body(_body)
                            .Return<TOut>()
                            .Compile<Func<TIn, TOut>>();
        }

        public TOut Execute<TIn, TOut>(TIn args)
        {
            var func = GetFunc<TIn, TOut>();
            return func(args);
        }

        public object Execute(object args)
        {
            return Execute<object, object>(args);
        }
    }
}
