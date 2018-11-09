namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class VariableReference : Place
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
