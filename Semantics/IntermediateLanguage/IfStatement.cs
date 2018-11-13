using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class IfStatement : BlockTerminatorStatement
    {
        public IfStatement(int blockNumber, int number)
            : base(blockNumber, number)
        {
        }

        public override IEnumerable<int> OutBlocks()
        {
            throw new NotImplementedException();
        }

        // Useful for debugging
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
