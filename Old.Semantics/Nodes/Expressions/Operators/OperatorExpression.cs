using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions.Operators
{
    public abstract class OperatorExpression : Expression
    {
        protected OperatorExpression([NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics, [NotNull]  DataType type)
            : base(diagnostics, type)
        {
        }
    }
}