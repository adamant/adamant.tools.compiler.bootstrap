using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
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
        public void BuildNamespaceScopesFor(PackageSyntax packageSyntax)
        {
            var packagesScope = BuildPackageScope(packageSyntax);


            // TODO finish building scopes
        }

        private static PackagesScope BuildPackageScope(PackageSyntax packageSyntax)
        {
            var packageAliases = packageSyntax.References
                                  .ToDictionary(p => p.Key, p => p.Value.Symbol)
                                  .ToFixedDictionary();
            return new PackagesScope(packageSyntax.Symbol, packageAliases);
        }

        private static GlobalScope<Promise<Symbol?>> BuildGlobalScope(PackageSyntax packageSyntax, PackagesScope _)
        {
            var nonMemberEntityDeclarations = packageSyntax.CompilationUnits
                                                           .SelectMany(cu => cu.AllEntityDeclarations)
                                                           .OfType<INonMemberEntityDeclarationSyntax>().ToList();

            var globalSymbolsInReferences = packageSyntax.References.Values
                                                         .SelectMany(p => p.SymbolTree.Children(p.Symbol))
                                                         .Where(s => !(s.Name is null))
                                                         .Select(s => (s.Name!, Symbol: Promise.ForValue(s)));

            //var globalSymbolsInPackage = nonMemberEntityDeclarations
            //                             .Where(e => e.ContainingNamespaceName == NamespaceName.Global)
            //                             .Select(e => (e.Name, e.Symbol));

            //globalSymbolsInPackage.Concat(globalSymbolsInReferences).ToFixedList()

            //var globalScope = new GlobalScope<Promise<Symbol?>>(packagesScope,);

            throw new NotImplementedException();
        }
    }
}
