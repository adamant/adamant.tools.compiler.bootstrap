using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public ICloseParenToken CloseParen { get; }

        public ParenthesizedExpressionSyntax(
            [NotNull] IOpenParenToken openParen,
            [NotNull] ExpressionSyntax expression,
            [NotNull] ICloseParenToken closeParen)
            : base(TextSpan.Covering(openParen.Span, closeParen.Span))
        {
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(expression), expression);
            Requires.NotNull(nameof(closeParen), closeParen);
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }
    }
}
