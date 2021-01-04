using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mirror.Workflows.Types
{
    public class TypeContainer : ITypeInfoProvider
    {
        private readonly ITypeInfoProvider _parent;
        private readonly ConcurrentDictionary<string, Type> _types;

        public TypeContainer(ITypeInfoProvider parent)
        {
            _parent = parent;
            _types = new ConcurrentDictionary<string, Type>();
        }

        public bool Add( Type type)
        {
            return _types.TryAdd(type.Name, type);
        }

        public Type Find(string name)
        {
    
            var type = _parent?.Find(name);
            if (type != null) return type;
            return _types.TryGetValue(name, out type) ? type : null;
        }

        public IEnumerable<string> ListNamespaces()
        {
            var parentNamespaces = _parent?.ListNamespaces();
            var currentNamespaces = _types.Values.Select(t => t.Namespace);
            if (parentNamespaces == null) return currentNamespaces;
            return parentNamespaces.Union(currentNamespaces);
        }

        public IEnumerable<Assembly> ListAssemblies()
        {
            var parentAssemblies = _parent?.ListAssemblies();
            var currentAssemblies = _types.Values.Select(t => t.Assembly);
            if (parentAssemblies == null) return currentAssemblies;
            return parentAssemblies.Union(currentAssemblies);
        }

   
    }
}