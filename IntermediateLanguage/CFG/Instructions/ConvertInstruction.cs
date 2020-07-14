using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class ConvertInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public NumericType FromType { get; }
        public NumericType ToType { get; }

        public ConvertInstruction(
            Place resultPlace,
            Operand operand,
            NumericType fromType,
            NumericType toType,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
            FromType = fromType;
            ToType = toType;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = CONVERT[{FromType},{ToType}] {Operand}";
        }
    }
}
