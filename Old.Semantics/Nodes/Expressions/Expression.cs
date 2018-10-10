using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions
{
    public abstract class Expression : SemanticNode
    {
        [NotNull]
        public DataType Type { get; }

        protected Expression([NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics, [NotNull] DataType type)
            : base(diagnostics)
        {
            Requires.NotNull(nameof(type), type);
            Type = type;
        }
    }
}
