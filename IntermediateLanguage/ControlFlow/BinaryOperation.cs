namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class BinaryOperation : Value
    {
        public Operand LeftOperand { get; }
        public BinaryOperator Operator { get; }
        public Operand RightOperand { get; }

        public BinaryOperation(Operand leftOperand, BinaryOperator @operator, Operand rightOperand)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator} {RightOperand}";
        }
    }
}
