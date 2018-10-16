namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues
{
    public class Dereference : LValue
    {
        public readonly LValue DereferencedValue;

        public Dereference(LValue dereferencedValue)
        {
            DereferencedValue = dereferencedValue;
        }

        public override int CoreVariable()
        {
            return DereferencedValue.CoreVariable();
        }

        public override string ToString()
        {
            return $"^{DereferencedValue}";
        }
    }
}
