using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        public Token ReturnKeyword { get; }
        public ExpressionSyntax Expression { get; }

        public ReturnExpressionSyntax(Token returnKeyword, ExpressionSyntax expression)
            : base(returnKeyword, expression)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }
    }
}
