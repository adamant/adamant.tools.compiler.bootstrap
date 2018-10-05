using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public SimpleToken OpenParen { get; }
        public ExpressionSyntax Expression { get; }
        public SimpleToken CloseParen { get; }

        public ParenthesizedExpressionSyntax(SimpleToken openParen, ExpressionSyntax expression, SimpleToken closeParen)
        {
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }
    }
}
