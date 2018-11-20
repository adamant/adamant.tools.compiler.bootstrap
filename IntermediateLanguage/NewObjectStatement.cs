using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class NewObjectStatement : ExpressionStatement
    {
        [NotNull] public readonly Place ResultInto;
        [NotNull] public readonly ObjectType Type;
        [NotNull] public FixedList<IValue> Arguments { get; }

        public NewObjectStatement(
            int blockNumber,
            int number,
            [NotNull] Place resultInto,
            [NotNull] ObjectType type,
            [NotNull] [ItemNotNull] IEnumerable<IValue> arguments)
            : base(blockNumber, number)
        {
            ResultInto = resultInto;
            Type = type;
            Arguments = arguments.ToFixedList();
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"{ResultInto} = new {Type}({string.Join(", ", Arguments)});";
        }
    }
}
