using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BreakExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull] public ExpressionSyntax Expression { get; }

        public BreakExpressionSyntax(
            TextSpan span,
            [CanBeNull] ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            if (Expression != null) return $"break {Expression}";
            return "break";
        }
    }
}
