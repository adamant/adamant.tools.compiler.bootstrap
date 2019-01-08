using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// A statement in a block
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Statement
    {
        public int BlockNumber { get; internal set; }
        public int Number { get; internal set; }

        // Useful for debugging
        public abstract override string ToString();

        public abstract Statement Clone();
    }
}
