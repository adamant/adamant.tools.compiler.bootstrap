using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class NewObjectInstruction : InstructionWithResult
    {
        public ConstructorSymbol Constructor { get; }
        /// <summary>
        /// The type being constructed
        /// </summary>
        public ObjectType ConstructedType { get; }
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public NewObjectInstruction(
            Place resultPlace,
            ConstructorSymbol constructor,
            ObjectType constructedType,
            FixedList<Operand> arguments,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Constructor = constructor;
            ConstructedType = constructedType;
            Arguments = arguments;
        }

        public override string ToInstructionString()
        {
            var result = ResultPlace != null ? $"{ResultPlace} = " : "";
            var arguments = string.Join(", ", Arguments);
            return $"{result}NEW {Constructor}({arguments})";
        }
    }
}
