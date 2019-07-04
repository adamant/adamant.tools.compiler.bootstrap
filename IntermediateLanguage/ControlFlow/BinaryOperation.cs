using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class BinaryOperation : Value
    {
        public IOperand LeftOperand { get; }
        public BinaryOperator Operator { get; }
        public IOperand RightOperand { get; }
        public SimpleType Type { get; }

        public BinaryOperation(
            IOperand leftOperand,
            BinaryOperator @operator,
            IOperand rightOperand,
            SimpleType type)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
            Type = type;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator.ToSymbolString()} {RightOperand}";
        }
    }
}
