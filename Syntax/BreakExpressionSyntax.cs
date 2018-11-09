using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BreakExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IBreakKeywordToken BreakKeyword { get; }
        [CanBeNull] public ExpressionSyntax Expression { get; }

        public BreakExpressionSyntax(
            [NotNull] IBreakKeywordToken breakKeyword,
            [CanBeNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(breakKeyword.Span, expression?.Span))
        {
            Requires.NotNull(nameof(breakKeyword), breakKeyword);
            BreakKeyword = breakKeyword;
            Expression = expression;
        }
    }
}
