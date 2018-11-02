namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class VariableReference : LValue
    {
        public readonly int VariableNumber;

        public VariableReference(int variableNumber)
        {
            VariableNumber = variableNumber;
        }

        public override string ToString()
        {
            return $"%{VariableNumber}";
        }

        public override int CoreVariable()
        {
            return VariableNumber;
        }
    }
}
