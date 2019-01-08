using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class AssignmentStatement : ExpressionStatement
    {
        public Place Place { get; }
        public Value Value { get; }

        public AssignmentStatement(Place place, Value value, TextSpan span)
            : base(span)
        {
            Place = place;
            Value = value;
        }

        public override Statement Clone()
        {
            return new AssignmentStatement(Place, Value, Span);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{Place} = {Value}";
        }
    }
}
