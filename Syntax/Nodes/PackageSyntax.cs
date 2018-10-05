using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PackageSyntax : SyntaxNode
    {
        public IReadOnlyList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(IEnumerable<CompilationUnitSyntax> compilationUnits)
        {
            CompilationUnits = compilationUnits.ToList().AsReadOnly();
        }
    }
}
