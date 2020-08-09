using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class PackagesScope<TSymbol> : LexicalScope<TSymbol>
    {
        internal override PackagesScope<TSymbol> ContainingPackagesScope => this;
        public PackageSymbol CurrentPackage { get; }
        private readonly FixedDictionary<Name, PackageSymbol> packageAliases;

        public PackagesScope(PackageSymbol currentPackage, FixedDictionary<Name, PackageSymbol> packageAliases)
        {
            CurrentPackage = currentPackage;
            this.packageAliases = packageAliases;
        }

        public override PackageSymbol? LookupPackage(Name name)
        {
            return packageAliases.TryGetValue(name, out var package) ? package : null;
        }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            return Enumerable.Empty<TSymbol>();
        }

        public override IEnumerable<TSymbol> Lookup(TypeName name, bool includeNested = true)
        {
            return Enumerable.Empty<TSymbol>();
        }
    }
}
