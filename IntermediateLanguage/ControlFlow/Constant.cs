using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public abstract class Constant : Operand
    {
        [NotNull] public readonly DataType Type;

        protected Constant([NotNull] DataType type)
        {
            Type = type;
        }
    }
}
