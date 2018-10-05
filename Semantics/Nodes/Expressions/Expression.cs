using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions
{
    public abstract class Expression : SemanticNode
    {
        public DataType Type { get; }

        protected Expression(IEnumerable<Diagnostic> diagnostics, DataType type)
            : base(diagnostics)
        {
            Type = type;
        }
    }
}
