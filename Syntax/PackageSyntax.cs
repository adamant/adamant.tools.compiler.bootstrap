using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class PackageSyntax
    {
        public IReadOnlyList<SyntaxTree<CompilationUnitSyntax>> SyntaxTrees { get; }

        public PackageSyntax(IEnumerable<SyntaxTree<CompilationUnitSyntax>> syntaxTrees)
        {
            SyntaxTrees = syntaxTrees.ToList().AsReadOnly();
        }
    }
}
