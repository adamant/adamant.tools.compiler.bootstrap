using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    public class LexicalScopesBuilder
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public void BuildFor(PackageSyntax package)
        {
            var declarationSymbols = GetAllNonMemberDeclarationSymbols(package);
            var namespaces = BuildNamespaces(declarationSymbols);
            var packagesScope = BuildPackagesScope(package);
            var globalScope = BuildGlobalScope(packagesScope, namespaces[NamespaceName.Global]);

            foreach (var compilationUnit in package.CompilationUnits)
            {
                var builder = new LexicalScopesBuilderWalker(globalScope, namespaces);
                builder.Walk(compilationUnit, globalScope);
            }
        }

        private static FixedList<NonMemberSymbol> GetAllNonMemberDeclarationSymbols(PackageSyntax package)
        {
            var primitiveSymbols = Primitive.SymbolTree.Symbols
                                            .Where(s => s.ContainingSymbol is null)
                                            .Select(s => new NonMemberSymbol(s));

            var packageNamespaces = package.SymbolTreeBuilder.Symbols
                                           .OfType<NamespaceSymbol>()
                                           .Select(s => new NonMemberSymbol(s));

            var packageSymbols = package.GetDeclarations()
                                        .OfType<INonMemberEntityDeclarationSyntax>()
                                        .Select(d => new NonMemberSymbol(d));

            // TODO it might be better to go to the declarations and get their symbols (once that is implemented)
            var referencedSymbols = package.References.Values
                                           .SelectMany(p => p.SymbolTree.Symbols)
                                           .Concat(Intrinsic.SymbolTree.Symbols)
                                           .Where(s => s.ContainingSymbol is NamespaceOrPackageSymbol)
                                           .Select(s => new NonMemberSymbol(s));
            return primitiveSymbols
                   .Concat(packageNamespaces)
                   .Concat(packageSymbols)
                   .Concat(referencedSymbols)
                   .ToFixedList();
        }

        private static FixedDictionary<NamespaceName, Namespace> BuildNamespaces(
            FixedList<NonMemberSymbol> declarationSymbols)
        {
            var namespaces = declarationSymbols.SelectMany(s => s.ContainingNamespace.NamespaceNames()).Distinct();
            var nsSymbols = new List<Namespace>();
            foreach (var ns in namespaces)
            {
                var symbols = declarationSymbols.Where(s => s.ContainingNamespace == ns).ToList();
                var nestedSymbols = declarationSymbols.Where(s => s.ContainingNamespace.IsNestedIn(ns)).ToList();

                nsSymbols.Add(new Namespace(
                    ns,
                    ToDictionary(symbols),
                    ToDictionary(nestedSymbols),
                    ToDictionary(symbols.Where(s => s.InCurrentPackage)),
                    ToDictionary(nestedSymbols.Where(s => s.InCurrentPackage))));
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

        private static NestedScope BuildGlobalScope(
            PackagesScope packagesScope,
            Namespace globalNamespace)
        {
            var allPackagesGlobalScope = NestedScope.CreateGlobal(packagesScope, globalNamespace.Symbols, globalNamespace.NestedSymbols);

            return allPackagesGlobalScope;
        }

        private static FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> ToDictionary(
            IEnumerable<NonMemberSymbol> symbols)
        {
            return symbols.GroupBy(s => s.Name, s => s.Symbol)
                          .ToFixedDictionary(e => e.Key, e => e.ToFixedSet());
        }
    }
}
