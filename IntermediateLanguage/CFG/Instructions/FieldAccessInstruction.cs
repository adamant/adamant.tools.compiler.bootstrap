using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class FieldAccessInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public FieldSymbol Field { get; }

        public FieldAccessInstruction(
            Place resultPlace,
            Operand operand,
            FieldSymbol field,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
            Field = field;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD[{Field.DataType}] {Operand}, {Field.ContainingSymbol}::{Field.Name}";
        }
    }
}
