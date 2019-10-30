using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Conversion : Value
    {
        public IOperand Operand { get; }
        public NumericType FromType { get; }
        public NumericType ToType { get; }
        public Conversion(IOperand operand, NumericType fromType, NumericType toType, TextSpan span)
            : base(span)
        {
            Operand = operand;
            FromType = fromType;
            ToType = toType;
        }

        public override string ToString()
        {
            return $"convert {FromType} to {ToType} ({Operand})";
        }
    }
}
