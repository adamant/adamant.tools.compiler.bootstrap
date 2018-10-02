using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class CompilationUnit : SemanticNode
    {
        public new CompilationUnitSyntax Syntax { get; }
        public CompilationUnitNamespaceDeclaration Namespace { get; }
        public IReadOnlyList<Declaration> Declarations { get; }

        public CompilationUnit(
            CompilationUnitSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            CompilationUnitNamespaceDeclaration @namespace,
            IEnumerable<Declaration> declarations)
            : base(diagnostics)
        {
            Syntax = syntax;
            Namespace = @namespace;
            Declarations = declarations.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics(IList<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            Syntax.AllDiagnostics(list);
            Namespace.AllDiagnostics(list);
            foreach (var declaration in Declarations)
                declaration.AllDiagnostics(list);
        }
    }
}
