using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public abstract class LexicalScope
    {
        private readonly SymbolSet symbols;
        private readonly SymbolSet nestedSymbols;

        protected LexicalScope(IEnumerable<ISymbol> symbols, IEnumerable<ISymbol> nestedSymbols)
        {
            this.symbols = new SymbolSet(symbols);
            this.nestedSymbols = new SymbolSet(nestedSymbols);
        }

        public virtual FixedList<ISymbol> Lookup(SimpleName name)
        {
            return symbols.TryGetValue(name, out var declaration)
                ? declaration
                : (nestedSymbols.TryGetValue(name, out var nestedDeclaration) ? nestedDeclaration : FixedList<ISymbol>.Empty);
        }

        public abstract FixedList<ISymbol> LookupGlobal(SimpleName name);
    }
}
