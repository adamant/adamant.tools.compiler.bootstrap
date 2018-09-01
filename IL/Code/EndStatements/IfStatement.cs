using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements
{
    public class IfStatement : EndStatement
    {
        public override IEnumerable<int> OutBlocks()
        {
            throw new NotImplementedException();
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine("if goto else goto");
        }
    }
}
