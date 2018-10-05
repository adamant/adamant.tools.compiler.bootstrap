using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class BlockSyntax : StatementSyntax
    {
        public SimpleToken OpenBrace { get; }
        public IReadOnlyList<StatementSyntax> Statements { get; }
        public SimpleToken CloseBrace { get; }

        public BlockSyntax(SimpleToken openBrace, IEnumerable<StatementSyntax> statements, SimpleToken closeBrace)
        {
            OpenBrace = openBrace;
            Statements = statements.ToList().AsReadOnly();
            CloseBrace = closeBrace;
        }
    }
}
