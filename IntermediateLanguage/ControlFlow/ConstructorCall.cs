using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ConstructorCall : Value
    {
        [NotNull] public readonly ObjectType Type;
        [NotNull] public FixedList<Value> Arguments { get; }

        public ConstructorCall(
            [NotNull] ObjectType type,
            [NotNull] [ItemNotNull] IEnumerable<Value> arguments)
        {
            Type = type;
            Arguments = arguments.ToFixedList();
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"new {Type}({string.Join(", ", Arguments)});";
        }
    }
}
