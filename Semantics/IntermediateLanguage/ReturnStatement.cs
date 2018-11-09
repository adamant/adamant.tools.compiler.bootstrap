using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class ReturnStatement : EndStatement
    {
        public override IEnumerable<int> OutBlocks()
        {
            return Enumerable.Empty<int>();
        }
    }
}
