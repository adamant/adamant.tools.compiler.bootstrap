using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class GotoStatement : BlockTerminatorStatement
    {
        public readonly int GotoBlockNumber;

        public GotoStatement(int gotoBlockNumber)
        {
            GotoBlockNumber = gotoBlockNumber;
        }

        public override IEnumerable<int> OutBlocks()
        {
            yield return GotoBlockNumber;
        }

        public override Statement Clone()
        {
            return new GotoStatement(GotoBlockNumber);
        }

        public override string ToString()
        {
            return $"goto bb{GotoBlockNumber}";
        }
    }
}
