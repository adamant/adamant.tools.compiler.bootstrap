using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public abstract class StatementSyntax : SyntaxBranchNode
    {
        protected StatementSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
