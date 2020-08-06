using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    public class PackageLexicalScopesBuilder
    {
        private readonly Diagnostics diagnostics;
        public PackageLexicalScopesBuilder(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public void BuildScopesFor(PackageSyntax packageSyntax)
        {
            var packagesScope = new PackagesScope(packageSyntax.Symbol, GetPackageAliases(packageSyntax.References));
            var globalScope = new GlobalScope<Promise<Symbol?>>(packagesScope);
            // TODO finish building scopes
        }

        private static FixedDictionary<Name, PackageSymbol> GetPackageAliases(FixedDictionary<Name, Package> references)
        {

            return references.ToDictionary(p => p.Key, p => p.Value.Symbol).ToFixedDictionary();
        }
    }
}
