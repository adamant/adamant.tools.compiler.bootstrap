using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public abstract class LexicalScope
    {
        private readonly SymbolSet symbols;

        protected LexicalScope(IEnumerable<ISymbol> symbols)
        {
            this.symbols = new SymbolSet(symbols);
        }

        public virtual FixedList<ISymbol> Lookup(SimpleName name)
        {
            return symbols.TryGetValue(name, out var declaration) ? declaration : FixedList<ISymbol>.Empty;
        }

        public abstract FixedList<ISymbol> LookupGlobal(SimpleName name);
    }
}
