namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class UnaryOperation : Value
    {
        public UnaryOperator Operator { get; }
        public Operand Operand { get; }

        public UnaryOperation(UnaryOperator @operator, Operand operand)
        {
            Operator = @operator;
            Operand = operand;
        }

        public override string ToString()
        {
            return $"{Operator} {Operand}";
        }
    }
}
