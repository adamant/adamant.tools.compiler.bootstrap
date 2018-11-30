using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class GotoStatement : BlockTerminatorStatement
    {
        public readonly int GotoBlockNumber;

        public GotoStatement(int blockNumber, int number, int gotoBlockNumber)
            : base(blockNumber, number)
        {
            GotoBlockNumber = gotoBlockNumber;
        }

        public override IEnumerable<int> OutBlocks()
        {
            yield return GotoBlockNumber;
        }

        public override string ToString()
        {
            return $"goto bb{GotoBlockNumber};";
        }
    }
}
