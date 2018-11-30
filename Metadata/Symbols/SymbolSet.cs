using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    // A set of symbols that are indexed by name
    public class SymbolSet : FixedDictionary<SimpleName, FixedList<ISymbol>>
    {
        [NotNull] public static new readonly SymbolSet Empty = new SymbolSet(Enumerable.Empty<ISymbol>());

        public SymbolSet([NotNull, ItemNotNull] IEnumerable<ISymbol> symbols)
        : base(GroupSymbols(symbols))
        {
        }

        [NotNull]
        private static Dictionary<SimpleName, FixedList<ISymbol>> GroupSymbols(
            [NotNull, ItemNotNull] IEnumerable<ISymbol> symbols)
        {
            return symbols.Distinct().GroupBy(s => s.LookupByName)
                .ToDictionary(g => g.Key, g => g.ToFixedList());
        }
    }
}
