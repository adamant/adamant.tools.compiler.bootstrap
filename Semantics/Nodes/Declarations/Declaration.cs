using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public abstract class Declaration : SemanticNode
    {
        protected Declaration(IEnumerable<DiagnosticInfo> diagnostics)
            : base(diagnostics)
        {
        }
    }
}
