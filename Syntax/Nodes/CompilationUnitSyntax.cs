using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxBranchNode
    {
        public IEnumerable<DeclarationSyntax> Declarations => Children.OfType<DeclarationSyntax>();

        public CompilationUnitSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
