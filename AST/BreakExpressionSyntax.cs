using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class BreakExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Value { get; }

        public BreakExpressionSyntax(
            TextSpan span,
            ExpressionSyntax value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value != null)
                return $"break {Value}";
            return "break";
        }
    }
}
