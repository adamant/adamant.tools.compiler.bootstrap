using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public class SymbolSet : IReadOnlyCollection<Symbol>
    {
        public static readonly SymbolSet Empty = new SymbolSet(Enumerable.Empty<Symbol>());

        private readonly FixedDictionary<SimpleName, FixedList<Symbol>> symbols;

        public int Count { get; }

        // TODO implement symbol set

        public SymbolSet(IEnumerable<Symbol> symbols)
        {
            this.symbols = GroupSymbols(symbols);
            Count = this.symbols.Sum(item => item.Value.Count);
        }

        private static FixedDictionary<SimpleName, FixedList<Symbol>> GroupSymbols(IEnumerable<Symbol> symbols)
        {
            return symbols.Distinct().GroupBy(LookupByName).ToFixedDictionary(g => g.Key, g => g.ToFixedList());
        }

        // TODO name this a property of Symbol?
        private static SimpleName LookupByName(Symbol symbol)
        {
            return symbol.FullName.UnqualifiedName.WithoutDeclarationNumber();
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return symbols.Values.SelectMany().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
