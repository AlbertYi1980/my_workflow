using System;
using System.Collections.Generic;

namespace Mirror.Workflows.Types
{
    public static class TypeNameNormalizer
    {
        private static readonly Dictionary<string, string> FriendlyTypes;

        static TypeNameNormalizer()
        {
            FriendlyTypes = new Dictionary<string, string>
            {
                { "bool", nameof(Boolean)},
                { "int", nameof(Int32)},
                { "string", nameof(String)},
                { "double", nameof(Double)},
                { "short", nameof(Int16)},
                { "long", nameof(Int64)},
                { "float", nameof(Single)},
                { "decimal", nameof(Decimal)},
            };
        }

        public static string Normalize(string friendlyNameOrName)
        {
            return FriendlyTypes.TryGetValue(friendlyNameOrName, out var name) ? name : null;
        }
    }
}