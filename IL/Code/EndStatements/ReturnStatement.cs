using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements
{
    public class ReturnStatement : EndStatement
    {
        public override IEnumerable<int> OutBlocks()
        {
            return Enumerable.Empty<int>();
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine("return");
        }
    }
}
