using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class NestedScope : LexicalScope
    {
        internal override PackagesScope ContainingPackagesScope { get; }
        private readonly LexicalScope containingScope;
        private readonly bool isGlobalScope;
        private readonly FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInScope;
        private readonly FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInNestedScopes;

        public static NestedScope Create(
            LexicalScope containingScope,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>>? symbolsInNestedScopes = null)
        {
            return new NestedScope(containingScope, false, symbolsInScope,
                symbolsInNestedScopes ?? FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>>.Empty);
        }

        public static NestedScope CreateGlobal(
            LexicalScope containingScope,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>>? symbolsInNestedScopes)
        {
            return new NestedScope(containingScope, true, symbolsInScope,
                symbolsInNestedScopes ?? FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>>.Empty);
        }

        private NestedScope(
            LexicalScope containingScope,
            bool isGlobalScope,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbolsInNestedScopes)
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
