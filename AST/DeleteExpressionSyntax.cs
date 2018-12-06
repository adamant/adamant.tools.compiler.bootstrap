using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class DeleteExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Expression { get; }

        public DeleteExpressionSyntax(TextSpan span, ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"delete {Expression}";
        }
    }
}
