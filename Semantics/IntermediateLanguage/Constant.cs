using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public abstract class Constant : Operand
    {
        [NotNull] public readonly DataType Type;

        protected Constant([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            Type = type;
        }
    }
}
