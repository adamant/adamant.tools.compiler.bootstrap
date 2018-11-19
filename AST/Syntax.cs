using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class Syntax
    {
        [NotNull] public abstract override string ToString();
    }
}
