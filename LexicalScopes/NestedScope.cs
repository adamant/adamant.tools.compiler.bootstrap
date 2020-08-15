using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using SymbolDictionary = Adamant.Tools.Compiler.Bootstrap.Framework.FixedDictionary<Adamant.Tools.Compiler.Bootstrap.Names.TypeName, Adamant.Tools.Compiler.Bootstrap.Framework.FixedSet<Adamant.Tools.Compiler.Bootstrap.Core.Promises.IPromise<Adamant.Tools.Compiler.Bootstrap.Symbols.Symbol>>>;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class NestedScope : LexicalScope
    {
        internal override PackagesScope ContainingPackagesScope { get; }
        private readonly LexicalScope containingScope;
        private readonly bool isGlobalScope;
        private readonly SymbolDictionary symbolsInScope;
        private readonly SymbolDictionary symbolsInNestedScopes;

        public static NestedScope Create(
            LexicalScope containingScope,
            SymbolDictionary symbolsInScope,
            SymbolDictionary? symbolsInNestedScopes = null)
        {
            return new NestedScope(containingScope, false, symbolsInScope,
                symbolsInNestedScopes ?? SymbolDictionary.Empty);
        }

        public static NestedScope CreateGlobal(
            LexicalScope containingScope,
            SymbolDictionary symbolsInScope,
            SymbolDictionary? symbolsInNestedScopes)
        {
            return new NestedScope(containingScope, true, symbolsInScope,
                symbolsInNestedScopes ?? SymbolDictionary.Empty);
        }

        private NestedScope(
            LexicalScope containingScope,
            bool isGlobalScope,
            SymbolDictionary symbolsInScope,
            SymbolDictionary symbolsInNestedScopes)
        {
            ContainingPackagesScope = containingScope.ContainingPackagesScope;
            this.containingScope = containingScope;
            this.isGlobalScope = isGlobalScope;
            this.symbolsInScope = symbolsInScope;
            this.symbolsInNestedScopes = symbolsInNestedScopes;
        }

        public override IEnumerable<IPromise<Symbol>> LookupInGlobalScope(TypeName name)
        {
            return !isGlobalScope ? containingScope.LookupInGlobalScope(name) : Lookup(name, false);
        }

        public override IEnumerable<IPromise<Symbol>> Lookup(TypeName name, bool includeNested = true)
        {
            if (symbolsInScope.TryGetValue(name, out var symbols)) return symbols;
            if (includeNested && symbolsInNestedScopes.TryGetValue(name, out symbols)) return symbols;
            return containingScope.Lookup(name, includeNested);
        }
    }
}
