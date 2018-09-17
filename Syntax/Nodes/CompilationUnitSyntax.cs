using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxBranchNode
    {
        public CompilationUnitNamespaceSyntax Namespace { get; }
        public IReadOnlyList<DeclarationSyntax> Declarations { get; }

        public CompilationUnitSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Namespace = Children.OfType<CompilationUnitNamespaceSyntax>().SingleOrDefault();
            Declarations = Children.OfType<DeclarationSyntax>().ToList().AsReadOnly();
        }
    }
}
