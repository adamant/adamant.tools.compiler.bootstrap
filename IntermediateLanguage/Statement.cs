using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    /// <summary>
    /// A statement in a block
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Statement
    {
        public readonly int BlockNumber;
        public readonly int Number;

        protected Statement(int blockNumber, int number)
        {
            BlockNumber = blockNumber;
            Number = number;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
