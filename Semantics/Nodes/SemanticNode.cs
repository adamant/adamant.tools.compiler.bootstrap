using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public abstract class SemanticNode
    {
        [NotNull] public SyntaxNode Syntax => GetSyntax();
        [NotNull] [ItemNotNull] public IReadOnlyList<Diagnostic> Diagnostics { get; }
        [NotNull] [ItemNotNull] private readonly List<Diagnostic> diagnostics;

        protected SemanticNode([NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics.ToList();
            Diagnostics = this.diagnostics.AsReadOnly();
        }

        [NotNull]
        protected abstract SyntaxNode GetSyntax();


        public virtual void AllDiagnostics([NotNull] List<Diagnostic> list)
        {
            foreach (var diagnostic in diagnostics)
                list.Add(diagnostic);
        }
    }
}
