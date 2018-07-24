using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class BlockSyntax : StatementSyntax
    {
        public BlockSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
