using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class CallStatement : Statement
    {
        [NotNull] public readonly Place Place;
        [NotNull] public readonly string FunctionName;
        [NotNull, ItemNotNull] public FixedList<IValue> Arguments { get; }

        public CallStatement(
            [NotNull] Place lvalue,
            [NotNull] string functionName,
            [NotNull, ItemNotNull] IEnumerable<IValue> arguments)
        {
            Place = lvalue;
            FunctionName = functionName;
            Arguments = arguments.ToFixedList();
        }
    }
}
