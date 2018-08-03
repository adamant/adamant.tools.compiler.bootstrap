using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public abstract class MemberDeclaration : Declaration
    {
        protected MemberDeclaration(IEnumerable<DiagnosticInfo> diagnostics)
            : base(diagnostics)
        {
        }
    }
}
