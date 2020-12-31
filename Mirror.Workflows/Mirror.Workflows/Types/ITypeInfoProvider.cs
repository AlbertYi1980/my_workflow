using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mirror.Workflows.Types
{
    public interface ITypeInfoProvider
    {
        Type Find(string name);
        IEnumerable<string> ListNamespaces();
        IEnumerable<Assembly> ListAssemblies();
    }
}