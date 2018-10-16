using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public abstract class EndStatement : Statement
    {
        public abstract IEnumerable<int> OutBlocks();
    }
}
