using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// A value is an rvalue. That is a value that can occur on the right side
    /// of an assignment.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Value : IValue
    {
        public TextSpan Span { get; }

        // Useful for debugging
        public abstract override string ToString();

        protected Value(TextSpan span)
        {
            Span = span;
        }
    }
}
