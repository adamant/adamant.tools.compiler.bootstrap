using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull]
        public OpenParenToken OpenParen { get; }

        [NotNull]
        public ExpressionSyntax Expression { get; }

        [CanBeNull]
        public CloseParenToken CloseParen { get; }

        public ParenthesizedExpressionSyntax(
            [CanBeNull] OpenParenToken openParen,
            [NotNull] ExpressionSyntax expression,
            [CanBeNull] CloseParenToken closeParen)
        {
            Requires.NotNull(nameof(expression), expression);
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }
    }
}
