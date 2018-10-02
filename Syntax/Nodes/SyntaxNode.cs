using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public abstract class SyntaxNode
    {
        public abstract CodeFile File { get; }
        public abstract TextSpan Span { get; }
        public abstract void AllDiagnostics(IList<Diagnostic> list);
    }
}
