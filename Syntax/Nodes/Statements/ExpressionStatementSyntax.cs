using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression { get; }
        public SimpleToken Semicolon { get; }

        public ExpressionStatementSyntax(ExpressionSyntax expression, in SimpleToken semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }
    }
}
