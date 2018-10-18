using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
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
        {
            Requires.NotNull(nameof(expression), expression);
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }
    }
}
