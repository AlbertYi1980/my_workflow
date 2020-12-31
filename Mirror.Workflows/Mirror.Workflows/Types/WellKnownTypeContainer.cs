using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mirror.Workflows.Types
{
    public class WellKnownTypeContainer : ITypeInfoProvider
    {
        private readonly Dictionary<string, Type> _types;

        public WellKnownTypeContainer()
        {
            _types = new Dictionary<string, Type>();
        
            var wellKnownTypes = new[]
            {
                typeof(int),
                typeof(string),
                typeof(double),
                typeof(float),
                typeof(decimal),
                typeof(short),
                typeof(long),
                typeof(byte),
                typeof(bool),
                typeof(TimeSpan),
                typeof(DateTime)
            };

            foreach (var wellKnownType in wellKnownTypes)
            {
                _types.Add(wellKnownType.Name, wellKnownType);
            }
        }

        public Type Find(string name)
        {
            return _types.TryGetValue(name, out var type) ? type : null;
        }

        public IEnumerable<string> ListNamespaces()
        {
            return _types.Values.Select(t => t.Namespace);
        }

        public IEnumerable<Assembly> ListAssemblies()
        {
            return _types.Values.Select(t => t.Assembly);
        }
    }
}