namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public abstract class LValue : RValue
    {
        public abstract override int CoreVariable();
    }
}
