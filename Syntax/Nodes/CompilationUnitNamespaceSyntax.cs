using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitNamespaceSyntax : SyntaxBranchNode
    {
        public NameSyntax Name { get; }

        public CompilationUnitNamespaceSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Name = Children.OfType<NameSyntax>().Single();
        }
    }
}
