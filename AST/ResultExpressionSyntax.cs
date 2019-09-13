using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class ResultExpressionSyntax : ExpressionBlockSyntax
    {
        public ExpressionSyntax Expression { get; }

        public ResultExpressionSyntax(
            TextSpan span,
            ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression}";
        }
    }
}
