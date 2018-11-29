using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public static class SymbolExtensions
    {
        public static bool IsGlobal([NotNull] this ISymbol symbol)
        {
            return symbol.FullName is SimpleName;
        }

        [NotNull]
        public static ISymbol Lookup([NotNull] this ISymbol symbol, [NotNull] SimpleName name)
        {
            if (symbol == UnknownSymbol.Instance) return UnknownSymbol.Instance;

            return symbol.ChildSymbols.TryGetValue(name, out var childSymbol)
                ? childSymbol
                : UnknownSymbol.Instance;
        }
    }
}
