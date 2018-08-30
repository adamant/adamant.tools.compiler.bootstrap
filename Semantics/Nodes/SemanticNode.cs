using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public abstract class SemanticNode
    {
        public SyntaxNode Syntax => GetSyntax();
        public IReadOnlyList<DiagnosticInfo> Diagnostics { get; }
        private readonly List<DiagnosticInfo> diagnostics;

        protected SemanticNode(IEnumerable<DiagnosticInfo> diagnostics)
        {
            this.diagnostics = diagnostics.ToList();
            Diagnostics = this.diagnostics.AsReadOnly();
        }

        protected abstract SyntaxNode GetSyntax();

        internal void AddDiagnostic(DiagnosticInfo diagnostic)
        {
            diagnostics.Add(diagnostic);
        }
    }
}
