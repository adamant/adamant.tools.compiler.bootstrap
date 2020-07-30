using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class NestedScope : LexicalScope
    {
        public LexicalScope ContainingScope { get; }

        public NestedScope(
            LexicalScope containingScope,
            SymbolSet symbols,
            SymbolSet? nestedSymbols = null)
            : base(symbols, nestedSymbols ?? SymbolSet.Empty)
        {
            ContainingScope = containingScope;
        }

        public NestedScope(LexicalScope containingScope, IEnumerable<ISymbol> symbols)
            : this(containingScope, new SymbolSet(symbols), SymbolSet.Empty)
        { }

        public NestedScope(LexicalScope containingScope, ISymbol symbol)
            : this(containingScope, new SymbolSet(symbol.Yield()), SymbolSet.Empty)
        { }

        public override FixedList<ISymbol> LookupInGlobalScope(Name name)
        {
            return ContainingScope.LookupInGlobalScope(name);
        }

        public override FixedList<ISymbol> Lookup(SimpleName name, bool includeNested = true)
        {
            var symbols = base.Lookup(name, includeNested);
            return symbols.Any() ? symbols : ContainingScope.Lookup(name, includeNested);
        }
    }
}
