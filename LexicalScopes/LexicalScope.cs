using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    /// <summary>
    /// Lookup things by name in lexical scopes
    /// </summary>
    public abstract class LexicalScope<TSymbol>
    {
        private readonly PackagesScope packagesScope;
        private readonly FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInScope;
        private readonly FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInNestedScopes;

        protected LexicalScope(
            PackagesScope packagesScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInNestedScopes)
        {
            this.packagesScope = packagesScope;
            this.symbolsInScope = symbolsInScope;
            this.symbolsInNestedScopes = symbolsInNestedScopes;
        }

        public PackageSymbol LookupPackage(Name name)
        {
            return packagesScope.LookupPackage(name);
        }

        public abstract IEnumerable<TSymbol> LookupInGlobalScope(TypeName name);

        public IEnumerable<TSymbol> Lookup(TypeName name, bool includeNested = true)
        {
            if (symbolsInScope.TryGetValue(name, out var symbols)) return symbols;
            if (includeNested && symbolsInNestedScopes.TryGetValue(name, out symbols)) return symbols;
            return Enumerable.Empty<TSymbol>();
        }

    }
}
