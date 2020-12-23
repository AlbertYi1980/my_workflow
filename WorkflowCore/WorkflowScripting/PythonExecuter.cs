﻿using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Reflection;

namespace WorkflowScripting
{
    public class PythonExecuter
    {
        private static readonly ScriptEngine _engine;
        private readonly ScriptScope _scope;        
        static PythonExecuter()
        {
            _engine = Python.CreateEngine();
        }
        public PythonExecuter()
        {            
            _scope = _engine.CreateScope();
        }

        public PythonExecuter Load(Assembly assembly)
        {
            _scope.Engine.Runtime.LoadAssembly(assembly);
            return this;
        }

        public PythonExecuter Inject(string name, object obj)
        {
            _scope.SetVariable(name, obj);
            return this;
        }

        public T Execute<T>(string code)
        {
            var ss = _engine.CreateScriptSourceFromString(code);
            var result = ss.Execute<T>(_scope);
            return result;
        }
    }
}
