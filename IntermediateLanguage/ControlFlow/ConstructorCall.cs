using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ConstructorCall : Value
    {
        public readonly UserObjectType Type;
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count + 1;

        public ConstructorCall(UserObjectType type, FixedList<Operand> arguments, TextSpan span)
            : base(span)
        {
            Type = type;
            Arguments = arguments;
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"new {Type}({string.Join(", ", Arguments)});";
        }
    }
}
