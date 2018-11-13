using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class ReturnStatement : BlockTerminatorStatement
    {
        public ReturnStatement(int blockNumber, int number)
            : base(blockNumber, number)
        {
        }

        public override IEnumerable<int> OutBlocks()
        {
            return Enumerable.Empty<int>();
        }

        public override string ToString()
        {
            return "return;";
        }
    }
}
