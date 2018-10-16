using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.RValues;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues
{
    public abstract class LValue : RValue
    {
        public abstract override int CoreVariable();
    }
}
