using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Declarations
{
    public abstract class DeclarationIL
    {
        internal abstract void ToString([NotNull] AsmBuilder builder);
    }
}
