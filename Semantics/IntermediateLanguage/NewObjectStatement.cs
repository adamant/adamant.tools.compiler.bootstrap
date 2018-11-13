using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class NewObjectStatement : Statement
    {
        [NotNull] public readonly Place ResultInto;
        [NotNull] public readonly ObjectType Type;
        [NotNull] public FixedList<IValue> Arguments { get; }

        public NewObjectStatement(
            [NotNull] Place resultInto,
            [NotNull] ObjectType type,
            [NotNull, ItemNotNull] IEnumerable<IValue> arguments)
        {
            ResultInto = resultInto;
            Type = type;
            Arguments = arguments.ToFixedList();
        }
    }
}
