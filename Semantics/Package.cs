using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class Package : SemanticNode
    {
        public new PackageSyntax Syntax { get; }
        public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
        public FunctionDeclaration EntryPoint { get; }
        // Name
        // References
        // Symbol

        public Package(PackageSyntax syntax, IEnumerable<CompilationUnit> compilationUnits, FunctionDeclaration entryPoint)
        {
            CompilationUnits = compilationUnits.ToList().AsReadOnly();
            Syntax = syntax;
            EntryPoint = entryPoint;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
