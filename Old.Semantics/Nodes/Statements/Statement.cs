using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Statements
{
    public abstract class Statement : SemanticNode
    {
        protected Statement(IEnumerable<Diagnostic> diagnostics)
            : base(diagnostics)
        {
        }
    }
}
