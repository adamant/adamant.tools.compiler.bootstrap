using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class GotoStatement : EndStatement
    {
        public readonly int BlockNumber;

        public GotoStatement(int blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public override IEnumerable<int> OutBlocks()
        {
            yield return BlockNumber;
        }
    }
}
