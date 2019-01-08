using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ActionStatement : ExpressionStatement
    {
        public Value Value { get; }

        public ActionStatement(Value value, TextSpan span)
            : base(span)
        {
            Value = value;
        }

        public override Statement Clone()
        {
            return new ActionStatement(Value, Span);
        }

        // Useful for debugging
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
