using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PackageSyntax : SyntaxBranchNode
    {
        public IReadOnlyList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(IEnumerable<CompilationUnitSyntax> compilationUnits)
            : base(compilationUnits)
        {
            CompilationUnits = Children.OfType<CompilationUnitSyntax>().ToList().AsReadOnly();
        }
    }
}
