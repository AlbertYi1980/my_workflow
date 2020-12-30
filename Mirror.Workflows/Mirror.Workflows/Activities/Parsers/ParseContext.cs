using Mirror.Workflows.TypeManagement;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class ParseContext
    {
        public ParseContext(CompositeActivityParser compositeParser, TypeContainer typeContainer)
        {
            CompositeParser = compositeParser;
            TypeContainer = typeContainer;
        }

        public CompositeActivityParser CompositeParser { get; }
        public TypeContainer TypeContainer { get; }
    }
}