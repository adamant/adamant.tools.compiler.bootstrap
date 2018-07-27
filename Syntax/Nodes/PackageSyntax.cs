using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PackageSyntax : SyntaxBranchNode
    {
        public IReadOnlyList<SyntaxTree<CompilationUnitSyntax>> SyntaxTrees { get; }

        public PackageSyntax(ICollection<SyntaxTree<CompilationUnitSyntax>> syntaxTrees)
            : base(syntaxTrees.Select(t => t.Root))
        {
            SyntaxTrees = syntaxTrees.ToList().AsReadOnly();
        }
    }
}
