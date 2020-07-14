using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    internal abstract class Syntax : ISyntax
    {
        public TextSpan Span { get; protected set; }

        protected Syntax(TextSpan span)
        {
            Span = span;
        }

        // This exists primarily for debugging use
        public abstract override string ToString();
    }
}
