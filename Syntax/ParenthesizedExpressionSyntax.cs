using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IOpenParenTokenPlace OpenParen { get; }
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public ICloseParenTokenPlace CloseParen { get; }

        public ParenthesizedExpressionSyntax(
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] ExpressionSyntax expression,
            [NotNull] ICloseParenTokenPlace closeParen)
            : base(TextSpan.Covering(openParen.Span, closeParen.Span))
        {
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(expression), expression);
            Requires.NotNull(nameof(closeParen), closeParen);
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}
