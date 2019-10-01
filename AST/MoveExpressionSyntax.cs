using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
    {
        public IExpressionSyntax Expression { get; }

        public MoveExpressionSyntax(TextSpan span, IExpressionSyntax expression)
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
