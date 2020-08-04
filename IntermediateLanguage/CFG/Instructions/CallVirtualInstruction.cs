using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class CallVirtualInstruction : Instruction
    {
        public Place? ResultPlace { get; }
        public MaybeQualifiedName Function { get; }

        public Operand Self { get; }

        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public CallVirtualInstruction(
            Place? resultPlace,
            Operand self,
            MaybeQualifiedName function,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
            Self = self;
            Function = function;
            Arguments = arguments;
        }

        public CallVirtualInstruction(
            Operand self,
            MaybeQualifiedName function,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : this(null, self, function, arguments, span, scope) { }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var arguments = string.Join(", ", Arguments);
            return $"{result}CALL.VIRT ({Self}).{Function}({arguments})";
        }
    }
}
