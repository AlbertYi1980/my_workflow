using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mirror.Workflows.TypeManagement
{
    public class TypesInfo
    {
        public TypesInfo(IEnumerable<Assembly> refAssemblies, IEnumerable<string> usingNamespaces, IEnumerable<Type> types)
        {
            RefAssemblies = refAssemblies;
            UsingNamespaces = usingNamespaces;
            Types = types;
        }

        public IEnumerable<Assembly> RefAssemblies { get;  }
        public IEnumerable<string> UsingNamespaces { get;  }
        
        public IEnumerable<Type> Types { get; }
    }

   
}