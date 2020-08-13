using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class CallInstruction : Instruction
    {
        public Place? ResultPlace { get; }
        public InvocableSymbol Function { get; }
        public Operand? Self { get; }
        public bool IsMethodCall => !(Self is null);
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        private CallInstruction(Place? resultPlace, Operand? self, InvocableSymbol function, FixedList<Operand> arguments, TextSpan span, Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
            Self = self;
            Function = function;
            Arguments = arguments;
        }

        public static CallInstruction ForFunction(
            Place resultPlace,
            FunctionSymbol function,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(resultPlace, null, function, arguments, span, scope);
        }

        public static CallInstruction ForFunction(
            FunctionSymbol function,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(null, null, function, arguments, span, scope);
        }

        public static CallInstruction ForMethod(
            Place resultPlace,
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(resultPlace, self, method, arguments, span, scope);
        }

        public static CallInstruction ForMethod(
            Operand self,
            MethodSymbol method,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
        {
            return new CallInstruction(null, self, method, arguments, span, scope);
        }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var selfArgument = Self?.ToString();
            if (!(selfArgument is null))
                selfArgument =  $"({selfArgument}).";
            var arguments = string.Join(", ", Arguments);
            var callType = IsMethodCall ? "METHOD" : "FN";
            return $"{result}CALL.{callType} {selfArgument}{Function}({arguments})";
        }
    }
}
