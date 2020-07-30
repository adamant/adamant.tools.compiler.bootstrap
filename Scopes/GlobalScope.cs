using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class GlobalScope : LexicalScope
    {
        public GlobalScope(IEnumerable<ISymbol> symbols, IEnumerable<ISymbol> nestedSymbols)
            : base(new SymbolSet(symbols), new SymbolSet(nestedSymbols))
        {
        }

        public override FixedList<ISymbol> LookupInGlobalScope(Name name)
        {
            // Don't include nested scopes, it must be in the global scope because it is global qualified
            return Lookup(name, includeNested: false);
        }
    }
}
