using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class BlockExpressionSyntax : ExpressionSyntax
    {
        public SimpleToken OpenBrace { get; }
        public SyntaxList<StatementSyntax> Statements { get; }
        public SimpleToken CloseBrace { get; }

        public BlockExpressionSyntax(SimpleToken openBrace, SyntaxList<StatementSyntax> statements, SimpleToken closeBrace)
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
    }
}
