using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class IntegerOperation : IValue
    {
        [NotNull] public readonly Operand LeftOperand;
        public readonly IntegerOperator Operator;
        public readonly bool IsChecked;
        [NotNull] public readonly Operand RightOperand;
        [NotNull] public readonly SizedIntegerType NumericType;

        private IntegerOperation(
            [NotNull] Operand leftOperand,
            IntegerOperator @operator,
            bool isChecked,
            [NotNull] Operand rightOperand,
            [NotNull] SizedIntegerType numericType)
        {
            Requires.NotNull(nameof(leftOperand), leftOperand);
            Requires.NotNull(nameof(rightOperand), rightOperand);
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            NumericType = numericType;
            Operator = @operator;
            IsChecked = isChecked;
        }

        public override string ToString()
        {
            var checking = IsChecked ? "Checked" : "Unchecked";
            var type = (NumericType.IsSigned ? "i" : "u") + NumericType.Bits;
            return $"{checking}_{Operator}_{type}({LeftOperand}, {RightOperand}";
        }
    }
}
