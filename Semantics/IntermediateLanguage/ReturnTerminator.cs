using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class ReturnTerminator : BlockTerminator
    {
        public override IEnumerable<int> OutBlocks()
        {
            return Enumerable.Empty<int>();
        }
    }
}
