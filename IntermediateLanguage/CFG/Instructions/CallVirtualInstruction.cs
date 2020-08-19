using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class CallVirtualInstruction : Instruction
    {
        public Place? ResultPlace { get; }
        public MethodSymbol Method { get; }
        public Operand Self { get; }
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public CallVirtualInstruction(
            Place? resultPlace,
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
            Self = self;
            Method = method;
            Arguments = arguments;
        }

        public CallVirtualInstruction(
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : this(null, self, method, arguments, span, scope) { }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var arguments = string.Join(", ", Arguments);
            return $"{result}CALL.VIRT({Self}).({arguments}) {Method}";
        }
    }
}
