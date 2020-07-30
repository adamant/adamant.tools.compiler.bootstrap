using System;
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
            return symbol switch
            {
                IParentSymbol parentSymbol =>
                    parentSymbol.ChildSymbols.TryGetValue(name, out var childSymbols)
                        ? childSymbols
                        : FixedList<ISymbol>.Empty,
                IBindingSymbol _ => FixedList<ISymbol>.Empty,
                _ => throw ExhaustiveMatch.Failed(symbol)
            };
        }

        public static T Assigned<T>(this T? symbol)
            where T : class, ISymbol
        {
            return symbol ?? throw new InvalidOperationException("Symbol not assigned");
        }
    }
}
