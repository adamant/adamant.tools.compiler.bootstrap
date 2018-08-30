namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class DeleteStatement : Statement
    {
        public readonly int VariableNumber;

        public DeleteStatement(int variableNumber)
        {
            VariableNumber = variableNumber;
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"delete %{VariableNumber}");
        }
    }
}
