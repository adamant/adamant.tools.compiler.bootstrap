using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MoveExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Expression { get; }

        public MoveExpressionSyntax(TextSpan span, ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"move {Expression}";
        }
    }
}
