using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public abstract class LexicalScope
    {
        [NotNull] private readonly SymbolSet symbols;

        protected LexicalScope([NotNull, ItemNotNull] IEnumerable<ISymbol> symbols)
        {
            this.symbols = new SymbolSet(symbols);
        }

        [NotNull]
        public virtual FixedList<ISymbol> Lookup([NotNull] SimpleName name)
        {
            return symbols.TryGetValue(name, out var declaration) ? declaration : FixedList<ISymbol>.Empty;
        }

        [NotNull]
        public abstract FixedList<ISymbol> LookupGlobal([NotNull] SimpleName name);
    }
}
