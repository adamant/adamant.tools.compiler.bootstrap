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
        public IReadOnlyList<Declaration> Declarations { get; }

        public CompilationUnit(
            CompilationUnitSyntax syntax,
            IEnumerable<Declaration> declarations,
            IEnumerable<DiagnosticInfo> diagnostics)
            : base(diagnostics)
        {
            Syntax = syntax;
            Declarations = declarations.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
