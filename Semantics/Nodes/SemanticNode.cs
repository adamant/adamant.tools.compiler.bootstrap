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

        protected SemanticNode(IEnumerable<DiagnosticInfo> diagnostics)
        {
            Diagnostics = diagnostics.ToList().AsReadOnly();
        }

        protected abstract SyntaxNode GetSyntax();
    }
}
