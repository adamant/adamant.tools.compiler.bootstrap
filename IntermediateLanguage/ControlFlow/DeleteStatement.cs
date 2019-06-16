using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class DeleteStatement : ExpressionStatement
    {
        public Place Place { get; }
        public ObjectType Type { get; }

        public DeleteStatement(Place place, ObjectType type, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Place = place;
            Type = type;
        }

        public override Statement Clone()
        {
            return new DeleteStatement(Place, Type, Span, Scope);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"delete {Place} // at {Span} in {Scope}";
        }
    }
}
