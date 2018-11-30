using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class CallStatement : ExpressionStatement
    {
        [NotNull] public readonly Place Place;
        [NotNull] public readonly Name FunctionName;
        [NotNull, ItemNotNull] public FixedList<IValue> Arguments { get; }

        public CallStatement(
            int blockNumber,
            int number,
            [NotNull] Place lvalue,
            [NotNull] Name functionName,
            [NotNull] [ItemNotNull] IEnumerable<IValue> arguments)
            : base(blockNumber, number)
        {
            Place = lvalue;
            FunctionName = functionName;
            Arguments = arguments.ToFixedList();
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{Place} = {FunctionName}({string.Join(",", Arguments)});";
        }
    }
}
