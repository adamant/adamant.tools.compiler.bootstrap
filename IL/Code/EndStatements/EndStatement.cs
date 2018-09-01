using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements
{
    public abstract class EndStatement : Statement
    {
        public abstract IEnumerable<int> OutBlocks();
    }
}
