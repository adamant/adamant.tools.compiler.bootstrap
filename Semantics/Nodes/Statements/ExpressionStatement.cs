using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class ExpressionStatement : Statement
    {
        public new ExpressionStatementSyntax Syntax { get; }
        public Expression Expression { get; }

        public ExpressionStatement(ExpressionStatementSyntax syntax, Expression expression)
        {
            Syntax = syntax;
            Expression = expression;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
