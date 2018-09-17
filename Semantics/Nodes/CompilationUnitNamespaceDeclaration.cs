using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class CompilationUnitNamespaceDeclaration : SemanticNode
    {
        public new CompilationUnitNamespaceSyntax Syntax { get; }

        public CompilationUnitNamespaceDeclaration(
            CompilationUnitNamespaceSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics)
            : base(diagnostics)
        {
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
