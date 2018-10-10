using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class CompilationUnit : SemanticNode
    {
        [NotNull] public new CompilationUnitSyntax Syntax { get; }
        [CanBeNull] public CompilationUnitNamespaceDeclaration Namespace { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Declaration> Declarations { get; }

        public CompilationUnit(
            [NotNull] CompilationUnitSyntax syntax,
            [NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [CanBeNull] CompilationUnitNamespaceDeclaration @namespace,
            [NotNull][ItemNotNull] IEnumerable<Declaration> declarations)
            : base(diagnostics)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Requires.NotNull(nameof(declarations), declarations);
            Syntax = syntax;
            Namespace = @namespace;
            Declarations = declarations.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics([NotNull] List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            Syntax.AllDiagnostics(list);
            Namespace?.AllDiagnostics(list);
            foreach (var declaration in Declarations)
                declaration.AllDiagnostics(list);
        }
    }
}
