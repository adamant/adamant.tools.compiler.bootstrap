using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// A statement that can end a block. These are statements that exit the block
    /// either to one or more other blocks or by returning from the function.
    /// </summary>
    public abstract class BlockTerminatorStatement : Statement
    {
        protected BlockTerminatorStatement(int blockNumber, int number)
            : base(blockNumber, number)
        {
        }

        public abstract IEnumerable<int> OutBlocks();
    }
}
