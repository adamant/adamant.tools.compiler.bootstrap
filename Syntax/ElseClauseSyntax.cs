using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ElseClauseSyntax : ExpressionSyntax
    {
        [NotNull] public ElseKeywordToken ElseKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public ElseClauseSyntax(
            [NotNull] ElseKeywordToken elseKeyword,
            [NotNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(elseKeyword.Span, expression.Span))
        {
            Requires.NotNull(nameof(elseKeyword), elseKeyword);
            Requires.NotNull(nameof(expression), expression);
            ElseKeyword = elseKeyword;
            Expression = expression;
        }
    }
}
