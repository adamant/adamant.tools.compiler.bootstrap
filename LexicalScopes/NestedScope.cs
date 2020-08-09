using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class NestedScope<TSymbol> : LexicalScope<TSymbol>
    {
        internal override PackagesScope<TSymbol> ContainingPackagesScope { get; }
        private readonly LexicalScope<TSymbol> containingScope;
        private readonly bool isGlobalScope;
        private readonly FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope;
        private readonly FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInNestedScopes;

        internal NestedScope(
            LexicalScope<TSymbol> containingScope,
            bool isGlobalScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInNestedScopes)
        {
            ContainingPackagesScope = containingScope.ContainingPackagesScope;
            this.containingScope = containingScope;
            this.isGlobalScope = isGlobalScope;
            this.symbolsInScope = symbolsInScope;
            this.symbolsInNestedScopes = symbolsInNestedScopes;
        }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            return !isGlobalScope ? containingScope.LookupInGlobalScope(name) : Lookup(name, false);
        }

        public override IEnumerable<TSymbol> Lookup(TypeName name, bool includeNested = true)
        {
            if (symbolsInScope.TryGetValue(name, out var symbols)) return symbols;
            if (includeNested && symbolsInNestedScopes.TryGetValue(name, out symbols)) return symbols;
            return containingScope.Lookup(name, includeNested);
        }
    }

    public static class NestedScope
    {
        public static NestedScope<TSymbol> Create<TSymbol>(
            LexicalScope<TSymbol> containingScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>>? symbolsInNestedScopes = null)
        {
            return new NestedScope<TSymbol>(containingScope, false, symbolsInScope,
                symbolsInNestedScopes ?? FixedDictionary<TypeName, FixedSet<TSymbol>>.Empty);
        }

        public static NestedScope<TSymbol> CreateGlobal<TSymbol>(
            LexicalScope<TSymbol> containingScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>>? symbolsInNestedScopes)
        {
            return new NestedScope<TSymbol>(containingScope, true, symbolsInScope,
                symbolsInNestedScopes ?? FixedDictionary<TypeName, FixedSet<TSymbol>>.Empty);
        }
    }
}
