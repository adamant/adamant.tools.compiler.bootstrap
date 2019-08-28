using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Syntax
    {
        private protected Syntax() { }

        // This exists primarily for debugging use
        public abstract override string ToString();
    }
}
