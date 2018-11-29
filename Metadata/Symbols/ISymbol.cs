using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public interface ISymbol
    {
        [NotNull] Name FullName { get; }
        [NotNull] DataType Type { get; }
        [NotNull] ISymbol ComposeWith([NotNull] ISymbol symbol);
        [CanBeNull] ISymbol Lookup([NotNull] SimpleName name);
    }
}
