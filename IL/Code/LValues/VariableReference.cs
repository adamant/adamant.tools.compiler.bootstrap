namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues
{
    public class VariableReference : LValue
    {
        public readonly int VariableNumber;

        public VariableReference(int variableNumber)
        {
            VariableNumber = variableNumber;
        }
    }
}
