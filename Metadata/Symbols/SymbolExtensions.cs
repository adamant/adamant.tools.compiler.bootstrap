using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public static class SymbolExtensions
    {
        [NotNull] private static readonly FixedList<ISymbol> UnknownSymbolList = new FixedList<ISymbol>(UnknownSymbol.Instance.Yield());

        public static bool IsGlobal([NotNull] this ISymbol symbol)
        {
            return symbol.FullName is SimpleName;
        }

        [NotNull]
        public static FixedList<ISymbol> Lookup([NotNull] this ISymbol symbol, [NotNull] SimpleName name)
        {
            if (symbol == UnknownSymbol.Instance) return UnknownSymbolList;

            return symbol.ChildSymbols.TryGetValue(name, out var childSymbols)
                    ? childSymbols
                    : FixedList<ISymbol>.Empty;
        }
    }
}
