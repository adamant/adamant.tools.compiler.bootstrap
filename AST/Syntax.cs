using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class Syntax
    {
        // This exists primarily for debugging use
        [NotNull] public abstract override string ToString();
    }
}
