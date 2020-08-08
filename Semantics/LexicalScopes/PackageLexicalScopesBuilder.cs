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
        public void BuildNamespaceScopesFor(PackageSyntax package)
        {
            var declarationSymbols = GetAllDeclarationSymbols(package);
            var namespaces = BuildNamespaces(declarationSymbols);
            var packagesScope = BuildPackagesScope(package);
            var globalScope = BuildGlobalScope(packagesScope, namespaces[NamespaceName.Global]);

            foreach (var compilationUnit in package.CompilationUnits)
            {
                var builder = new NamespaceLexicalScopesBuilder(diagnostics, namespaces);
                //new SyntaxScopesBuilder(compilationUnit.CodeFile, GlobalScope, namespaces, diagnostics);
                builder.Walk(compilationUnit, globalScope);
            }
        }

        private static FixedList<DeclarationSymbol> GetAllDeclarationSymbols(PackageSyntax package)
        {
            var packageSymbols = package.CompilationUnits
                                        .SelectMany(cu => cu.AllEntityDeclarations)
                                        .OfType<INonMemberEntityDeclarationSyntax>()
                                        .Select(d => new DeclarationSymbol(d));

            // TODO it might be better to go to the declarations and get their symbols (once that is implemented)
            var referencedSymbols = package.References.Values
                                           .SelectMany(p => p.SymbolTree.Symbols)
                                           .Where(s => s.ContainingSymbol is NamespaceOrPackageSymbol)
                                           .Select(s => new DeclarationSymbol(s));
            return packageSymbols.Concat(referencedSymbols).ToFixedList();
        }

        private static FixedDictionary<NamespaceName, Namespace> BuildNamespaces(
            FixedList<DeclarationSymbol> declarationSymbols)
        {
            var namespaces = declarationSymbols.SelectMany(s => s.ContainingNamespace.NamespaceNames()).Distinct();
            var nsSymbols = new List<Namespace>();
            foreach (var ns in namespaces)
            {
                var symbols = ToDictionary(declarationSymbols.Where(s => s.ContainingNamespace == ns));
                var nestedSymbols = ToDictionary(declarationSymbols.Where(s => s.ContainingNamespace.IsNestedIn(ns)));
                nsSymbols.Add(new Namespace(ns, symbols, nestedSymbols));
            }

            return nsSymbols.ToFixedDictionary(ns => ns.Name);
        }
        private static PackagesScope BuildPackagesScope(PackageSyntax package)
        {
            var packageAliases = package.References
                                  .ToDictionary(p => p.Key, p => p.Value.Symbol)
                                  .ToFixedDictionary();
            return new PackagesScope(package.Symbol, packageAliases);
        }

        private static GlobalScope<Promise<Symbol?>> BuildGlobalScope(
            PackagesScope packagesScope,
            Namespace globalNamespace)
        {
            return new GlobalScope<Promise<Symbol?>>(packagesScope, globalNamespace.Symbols, globalNamespace.NestedSymbols);
        }

        private static FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> ToDictionary(
            IEnumerable<DeclarationSymbol> symbols)
        {
            return symbols.GroupBy(s => s.Name, s => s.Symbol)
                          .ToFixedDictionary(e => e.Key, e => e.ToFixedList());
        }
    }
}
