using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols
{
    public interface ISymbol
    {
        [NotNull] Name FullName { get; }
        [NotNull] DataType Type { get; }
        [NotNull] ISymbol ComposeWith([NotNull] ISymbol symbol);
        [CanBeNull] ISymbol Lookup([NotNull] SimpleName name);
    }
}
