using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

using System.Reflection;

namespace WorkflowScripting
{
    public class PythonExecutor
    {
        private static readonly ScriptEngine _engine;
        private readonly ScriptScope _scope;        
        static PythonExecutor()
        {
            _engine = Python.CreateEngine();
        }
        public PythonExecutor()
        {            
            _scope = _engine.CreateScope();
        }

        public PythonExecutor Load(Assembly assembly)
        {
            _scope.Engine.Runtime.LoadAssembly(assembly);
            return this;
        }

        public PythonExecutor Inject(string name, object obj)
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
