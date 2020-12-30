using System;
using System.Activities;
using System.Text.Json;

namespace Mirror.Workflows.Activities.Parsers
{
    public class ActivityDescriptor
    {
        public string Name { get; }
        public Func<JsonElement, Activity> Parser { get; }

        public ActivityDescriptor(string name, Func<JsonElement, Activity> parser)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }
    }
}