using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class AssignmentStatement : ExpressionStatement
    {
        public Place Place { get; }
        public Value Value { get; }

        public AssignmentStatement(Place place, Value value, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Place = place;
            Value = value;
        }

        public override Statement Clone()
        {
            return new AssignmentStatement(Place, Value, Span, Scope);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{Place} = {Value} // at {Span} in {Scope}";
        }
    }
}
