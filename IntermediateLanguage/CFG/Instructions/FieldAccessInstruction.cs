using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class FieldAccessInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public SimpleName FieldName { get; }

        public FieldAccessInstruction(
            Place resultPlace,
            Operand operand,
            SimpleName fieldName,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
            FieldName = fieldName;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operand}.{FieldName}";
        }
    }
}
