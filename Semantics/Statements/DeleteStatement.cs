namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class DeleteStatement : SimpleStatement
    {
        public readonly int VariableNumber;

        public DeleteStatement(int variableNumber)
        {
            VariableNumber = variableNumber;
        }
    }
}
