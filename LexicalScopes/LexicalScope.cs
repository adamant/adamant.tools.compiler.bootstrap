using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    /// <summary>
    /// Lookup things by name in lexical scopes
    /// </summary>
    public abstract class LexicalScope<TSymbol>
    {
        internal abstract PackagesScope<TSymbol> ContainingPackagesScope { get; }

        public virtual PackageSymbol? LookupPackage(Name name)
        {
            return ContainingPackagesScope.LookupPackage(name);
        }

        public abstract IEnumerable<TSymbol> LookupInGlobalScope(TypeName name);

        public abstract IEnumerable<TSymbol> Lookup(TypeName name, bool includeNested = true);
    }
}
