using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public abstract class SemanticNode
    {
        public SyntaxNode Syntax => GetSyntax();
        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        private readonly List<Diagnostic> diagnostics;

        protected SemanticNode(IEnumerable<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics.ToList();
            Diagnostics = this.diagnostics.AsReadOnly();
        }

        protected abstract SyntaxNode GetSyntax();

        internal void AddDiagnostic(Diagnostic diagnostic)
        {
            diagnostics.Add(diagnostic);
        }

        public virtual void AllDiagnostics(List<Diagnostic> list)
        {
            foreach (var diagnostic in diagnostics)
                list.Add(diagnostic);
        }
    }
}
