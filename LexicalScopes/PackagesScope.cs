using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class PackagesScope
    {
        public PackageSymbol CurrentPackage { get; }
        private readonly FixedDictionary<Name, PackageSymbol> packageAliases;

        public PackagesScope(PackageSymbol currentPackage, FixedDictionary<Name, PackageSymbol> packageAliases)
        {
            CurrentPackage = currentPackage;
            this.packageAliases = packageAliases;
        }

        // No containing scope
        // List of package names with there package global scopes

        public PackageSymbol LookupPackage(Name name)
        {
            throw new NotImplementedException();
        }
    }
}
