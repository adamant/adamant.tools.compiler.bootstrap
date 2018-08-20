namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class DropStatement : Statement
    {
        public readonly int VariableNumber;

        public DropStatement(int variableNumber)
        {
            VariableNumber = variableNumber;
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"drop %{VariableNumber}");
        }
    }
}
