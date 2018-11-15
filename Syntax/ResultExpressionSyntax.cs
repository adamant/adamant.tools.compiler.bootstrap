using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ResultExpressionSyntax : ExpressionBlockSyntax
    {
        [NotNull] public IEqualsGreaterThanToken EqualsGreaterThan { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public ResultExpressionSyntax(
            [NotNull] IEqualsGreaterThanToken equalsGreaterThan,
            [NotNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(equalsGreaterThan.Span, expression.Span))
        {
            EqualsGreaterThan = equalsGreaterThan;
            Expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression}";
        }
    }
}
