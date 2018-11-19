using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// Note: Naming this `Syntax` would conflict with the `Syntax` namespace
    public abstract class NonTerminal
    {
        [NotNull] public abstract override string ToString();
    }
}
