using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UnsafeExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IUnsafeKeywordToken UnsafeKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public UnsafeExpressionSyntax(
            [NotNull] IUnsafeKeywordToken unsafeKeyword,
            [NotNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(unsafeKeyword.Span, expression.Span))
        {
            Requires.NotNull(nameof(unsafeKeyword), unsafeKeyword);
            Requires.NotNull(nameof(expression), expression);
            UnsafeKeyword = unsafeKeyword;
            Expression = expression;
        }
    }
}
