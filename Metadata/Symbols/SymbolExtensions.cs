using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public static class SymbolExtensions
    {
        private static readonly FixedList<ISymbol> UnknownSymbolList = new FixedList<ISymbol>(UnknownSymbol.Instance.Yield());

        public static bool IsGlobal(this ISymbol symbol)
        {
            return symbol.FullName is SimpleName;
        }

        public static FixedList<ISymbol> Lookup(this ISymbol symbol, SimpleName name)
        {
            if (symbol == UnknownSymbol.Instance) return UnknownSymbolList;
            switch (symbol)
            {
                default:
                    throw ExhaustiveMatch.Failed(symbol);
                case IParentSymbol parentSymbol:
                    return parentSymbol.ChildSymbols.TryGetValue(name, out var childSymbols)
                        ? childSymbols
                        : FixedList<ISymbol>.Empty;
                case IBindingSymbol _:
                    return FixedList<ISymbol>.Empty;
            }
        }
    }
}
