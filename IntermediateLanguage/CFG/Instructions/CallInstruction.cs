using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class CallInstruction : Instruction
    {
        public Place? ResultPlace { get; }
        public Name Function { get; }
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public CallInstruction(Place? resultPlace, Name function, FixedList<Operand> arguments, TextSpan span, Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
            Function = function;
            Arguments = arguments;
        }
        public CallInstruction(Name function, FixedList<Operand> arguments, TextSpan span, Scope scope)
            : this(null, function, arguments, span, scope)
        {
        }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var arguments = string.Join(", ", Arguments);
            return $"{result}CALL.FN {Function}({arguments})";
        }
    }
}
