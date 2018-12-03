using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FunctionCall : Value
    {
        [NotNull] public readonly Name FunctionName;
        [NotNull, ItemNotNull] public FixedList<Operand> Arguments { get; }

        public FunctionCall(
            [NotNull] Name functionName,
            [NotNull, ItemNotNull] IEnumerable<Operand> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments.ToFixedList();
        }

        public FunctionCall(
            [NotNull] Name functionName,
            [NotNull, ItemNotNull] params Operand[] arguments)
            : this(functionName, arguments as IEnumerable<Operand>)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{FunctionName}({string.Join(",", Arguments)});";
        }
    }
}
