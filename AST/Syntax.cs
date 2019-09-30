using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{

    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Syntax : ISyntax
    {
        public TextSpan Span { get; }

        protected Syntax(TextSpan span)
        {
            Span = span;
        }

        // This exists primarily for debugging use
        public abstract override string ToString();
    }
}
