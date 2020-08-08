using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        public void BuildNamespaceScopesFor(PackageSyntax packageSyntax)
        {
            var packagesScope = BuildPackagesScope(packageSyntax);
            var globalScope = BuildGlobalScope(packageSyntax, packagesScope);

            // TODO finish building scopes
        }

        private static PackagesScope BuildPackagesScope(PackageSyntax packageSyntax)
        {
            var packageAliases = packageSyntax.References
                                  .ToDictionary(p => p.Key, p => p.Value.Symbol)
                                  .ToFixedDictionary();
            return new PackagesScope(packageSyntax.Symbol, packageAliases);
        }

        private static GlobalScope<Promise<Symbol?>> BuildGlobalScope(PackageSyntax packageSyntax, PackagesScope packagesScope)
        {
            var nonMemberEntityDeclarations = packageSyntax.CompilationUnits
                                                           .SelectMany(cu => cu.AllEntityDeclarations)
                                                           .OfType<INonMemberEntityDeclarationSyntax>().ToList();

            var globalSymbols = GlobalSymbols(packageSyntax, nonMemberEntityDeclarations);
            var nonGlobalSymbols = NonGlobalSymbols(packageSyntax, nonMemberEntityDeclarations);
            return new GlobalScope<Promise<Symbol?>>(packagesScope, globalSymbols, nonGlobalSymbols);
        }

        private static FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> GlobalSymbols(
            PackageSyntax packageSyntax,
            IEnumerable<INonMemberEntityDeclarationSyntax> nonMemberEntityDeclarations)
        {
            var symbolsInPackage = GetNameAndSymbol(nonMemberEntityDeclarations
                                    .Where(e => e.ContainingNamespaceName == NamespaceName.Global));

            var symbolsInReferences = GetNameAndSymbol(packageSyntax.References.Values
                                    .SelectMany(p => p.SymbolTree.Children(p.Symbol)));

            return ToDictionary(symbolsInPackage.Concat(symbolsInReferences));
        }

        private static FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> NonGlobalSymbols(
            PackageSyntax packageSyntax,
            List<INonMemberEntityDeclarationSyntax> nonMemberEntityDeclarations)
        {
            var symbolsInPackage = GetNameAndSymbol(nonMemberEntityDeclarations
                                    .Where(e => e.ContainingNamespaceName != NamespaceName.Global));

            var symbolsInReferences = GetNameAndSymbol(packageSyntax.References.Values
                                    .SelectMany(p => p.SymbolTree.Symbols)
                                    .Where(s => s.ContainingSymbol is NamespaceSymbol));

            return ToDictionary(symbolsInPackage.Concat(symbolsInReferences));
        }

        private static IEnumerable<NameAndSymbol> GetNameAndSymbol(
            IEnumerable<INonMemberEntityDeclarationSyntax> syntax)
        {
            return syntax.Where(s => !(s.Name is null)).Select(s => new NameAndSymbol(s.Name!, s.Symbol));
        }

        private static IEnumerable<NameAndSymbol> GetNameAndSymbol(IEnumerable<Symbol> symbols)
        {
            return symbols.Where(s => !(s.Name is null)).Select(s => new NameAndSymbol(s.Name!, s));
        }

        private static FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> ToDictionary(
            IEnumerable<NameAndSymbol> symbols)
        {
            return symbols.GroupBy(s => s.Name, s => s.Symbol)
                          .ToFixedDictionary(e => e.Key, e => e.ToFixedList());
        }
    }
}
