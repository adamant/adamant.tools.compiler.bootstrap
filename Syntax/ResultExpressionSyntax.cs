using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ResultExpressionSyntax : ExpressionBlockSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }

        public ResultExpressionSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax expression)
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
