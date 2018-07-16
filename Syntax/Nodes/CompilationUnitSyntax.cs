using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxBranchNode
    {
        public CompilationUnitSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
