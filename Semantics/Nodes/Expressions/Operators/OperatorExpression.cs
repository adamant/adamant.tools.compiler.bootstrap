using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    public abstract class OperatorExpression : Expression
    {
        protected OperatorExpression(IEnumerable<DiagnosticInfo> diagnostics, DataType type)
            : base(diagnostics, type)
        {
        }
    }
}
