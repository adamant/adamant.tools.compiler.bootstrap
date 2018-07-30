using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class Package : SemanticNode
    {
        public new PackageSyntax Syntax { get; }
        public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
        public FunctionDeclaration EntryPoint { get; }
        // Name
        // References
        // Symbol

        public Package(
            PackageSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            IEnumerable<CompilationUnit> compilationUnits,
            FunctionDeclaration entryPoint)
            : base(diagnostics)
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
