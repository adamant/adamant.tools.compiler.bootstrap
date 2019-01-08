using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class DeleteStatement : ExpressionStatement
    {
        public Place Place { get; }
        public ObjectType Type { get; }

        public DeleteStatement(Place place, ObjectType type, TextSpan span)
            : base(span)
        {
            Place = place;
            Type = type;
        }

        public override Statement Clone()
        {
            return new DeleteStatement(Place, Type, Span);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"delete {Place} // at {Span}";
        }
    }
}
