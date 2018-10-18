using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    public class Package : SemanticNode
    {
        [NotNull] public new PackageSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
        [CanBeNull] public FunctionDeclaration EntryPoint { get; }
        // Name
        // References
        // Symbol

        public Package(
            [NotNull] PackageSyntax syntax,
            [NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [NotNull][ItemNotNull] IEnumerable<CompilationUnit> compilationUnits,
            [CanBeNull] FunctionDeclaration entryPoint)
            : base(diagnostics)
        {
            CompilationUnits = compilationUnits.ToReadOnlyList();
            Syntax = syntax;
            EntryPoint = entryPoint;
        }

        [NotNull]
        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        [NotNull]
        [ItemNotNull]
        public IList<Diagnostic> AllDiagnostics()
        {
            var diagnostics = new List<Diagnostic>();
            foreach (var compilationUnit in CompilationUnits)
                compilationUnit.AllDiagnostics(diagnostics);

            return diagnostics;
        }
    }
}
