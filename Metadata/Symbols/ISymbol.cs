using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public interface ISymbol
    {
        [NotNull] Name FullName { get; }
        [NotNull] DataType Type { get; }
        [NotNull] FixedDictionary<SimpleName, ISymbol> ChildSymbols { get; }
    }
}
