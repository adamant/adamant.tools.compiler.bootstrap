using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class GotoStatement : BlockTerminator
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
