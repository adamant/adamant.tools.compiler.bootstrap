using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class BlockSyntax : StatementSyntax
    {
        public IEnumerable<StatementSyntax> Statements => Children.OfType<StatementSyntax>();

        public BlockSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
