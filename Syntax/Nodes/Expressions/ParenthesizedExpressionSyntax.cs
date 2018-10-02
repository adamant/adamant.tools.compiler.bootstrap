using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public Token OpenParen { get; }
        public ExpressionSyntax Expression { get; }
        public Token CloseParen { get; }

        public ParenthesizedExpressionSyntax(Token openParen, ExpressionSyntax expression, Token closeParen)
            : base(openParen, expression, closeParen)
        {
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }
    }
}
