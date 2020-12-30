using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mirror.Workflows.TypeManagement
{
    public class TypesInfoProvider : ITypesProvider
    {
        private readonly TypesInfo _staticInfo;
        private readonly string _dynamicAssemblyName;
        private readonly string _code;

        public TypesInfoProvider(TypesInfo staticInfo, string dynamicAssemblyName, string code)
        {
            _staticInfo = staticInfo;
            _dynamicAssemblyName = dynamicAssemblyName;
            _code = code;
        }
        public TypesInfo Get()
        {
            var refAssemblies = new List<Assembly>();
            var usingNamespaces = new List<string>();
            var types = new List<Type>();
            if (_staticInfo != null)
            {
                refAssemblies .AddRange(_staticInfo.RefAssemblies);
                usingNamespaces.AddRange(_staticInfo.UsingNamespaces);
                types.AddRange(_staticInfo.Types);
            }

            if (_dynamicAssemblyName != null && _code != null)
            {
                var assembly = DynamicModelLoader.Load(_dynamicAssemblyName, _code);
                refAssemblies.Add(assembly);
                usingNamespaces.AddRange(assembly.GetTypes().Select(t => t.Namespace).Distinct());
                types.AddRange(assembly.GetTypes());
            }
            return new TypesInfo(refAssemblies, usingNamespaces, types);
        }
    }
}