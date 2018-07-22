using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class Package : SemanticNode
    {
        public new PackageSyntax Syntax { get; }
        public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
        // Name
        // References
        // Symbol

        public Package(PackageSyntax syntax, IEnumerable<CompilationUnit> compilationUnits)
        {
            CompilationUnits = compilationUnits.ToList().AsReadOnly();
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
