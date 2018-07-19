using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class PackageSyntax
    {
        public IReadOnlyList<SyntaxTree> SyntaxTrees { get; }

        public PackageSyntax(IEnumerable<SyntaxTree> syntaxTrees)
        {
            SyntaxTrees = syntaxTrees.ToList().AsReadOnly();
        }
    }
}
