using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class PackagesScope
    {
        // No containing scope
        // List of package names with there package global scopes

        public PackageSymbol LookupPackage(SimpleName name)
        {
            throw new NotImplementedException();
        }
    }
}
