using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class BlockSyntax : StatementSyntax
    {
        public SimpleToken OpenBrace { get; }
        public SyntaxList<StatementSyntax> Statements { get; }
        public SimpleToken CloseBrace { get; }

        public BlockSyntax(SimpleToken openBrace, SyntaxList<StatementSyntax> statements, SimpleToken closeBrace)
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
    }
}
