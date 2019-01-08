using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ReturnStatement : BlockTerminatorStatement
    {
        public override IEnumerable<int> OutBlocks()
        {
            return Enumerable.Empty<int>();
        }

        public override Statement Clone()
        {
            return new ReturnStatement();
        }

        public override string ToString()
        {
            return "return";
        }
    }
}
