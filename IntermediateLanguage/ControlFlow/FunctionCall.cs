using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FunctionCall : Value
    {
        public readonly Name FunctionName;
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public FunctionCall(
            Name functionName,
            IEnumerable<Operand> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments.ToFixedList();
        }

        public FunctionCall(
            Name functionName,
            params Operand[] arguments)
            : this(functionName, arguments as IEnumerable<Operand>)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"call {FunctionName}({string.Join(",", Arguments)});";
        }
    }
}
