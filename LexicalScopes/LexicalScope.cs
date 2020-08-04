using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<SimpleName, List<TSymbol>> symbolsInScope = new Dictionary<SimpleName, List<TSymbol>>();
        private readonly Dictionary<SimpleName, List<TSymbol>> symbolsInNestedScopes = new Dictionary<SimpleName, List<TSymbol>>();

        protected LexicalScope(PackagesScope packagesScope)
        {
            this.packagesScope = packagesScope;
        }

        public PackageSymbol LookupPackage(SimpleName name)
        {
            return packagesScope.LookupPackage(name);
        }

        public abstract IEnumerable<TSymbol> LookupInGlobalScope(SimpleName name);

        public IEnumerable<TSymbol> Lookup(SimpleName name, bool includeNested = true)
        {
            if (symbolsInScope.TryGetValue(name, out var symbols)) return symbols;
            if (includeNested && symbolsInNestedScopes.TryGetValue(name, out symbols)) return symbols;
            return Enumerable.Empty<TSymbol>();
        }

    }
}
