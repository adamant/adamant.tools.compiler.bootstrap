using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// A statement in a block
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Statement
    {
        public BasicBlockName Block { get; internal set; }
        public int Number { get; internal set; }

        public Scope Scope { get; }
        public TextSpan Span { get; }

        protected Statement(TextSpan span, Scope scope)
        {
            Span = span;
            Scope = scope;
        }

        public abstract Statement Clone();

        // Useful for debugging
        public abstract override string ToString();
    }
}
