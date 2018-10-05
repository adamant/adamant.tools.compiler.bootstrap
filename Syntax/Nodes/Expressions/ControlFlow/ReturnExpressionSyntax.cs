using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        public SimpleToken ReturnKeyword { get; }
        public ExpressionSyntax Expression { get; }

        public ReturnExpressionSyntax(SimpleToken returnKeyword, ExpressionSyntax expression)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }
    }
}
