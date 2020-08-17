using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Namespaces
{
    public class NamespaceSymbolBuilder : SyntaxWalker<NamespaceOrPackageSymbol>
    {
        private readonly SymbolTreeBuilder treeBuilder;

        private NamespaceSymbolBuilder(SymbolTreeBuilder treeBuilder)
        {
            this.treeBuilder = treeBuilder;
        }

        public static void BuildNamespaceSymbols(PackageSyntax package)
        {
            var builder = new NamespaceSymbolBuilder(package.SymbolTree);
            foreach (var compilationUnit in package.CompilationUnits)
                builder.Walk(compilationUnit, package.Symbol);
        }

        protected override void WalkNonNull(ISyntax syntax, NamespaceOrPackageSymbol containingSymbol)
        {
            switch (syntax)
            {
                case ICompilationUnitSyntax syn:
                {
                    var sym = BuildNamespaceSymbol(containingSymbol, syn.ImplicitNamespaceName);
                    WalkChildren(syn, sym);
                }
                break;
                case INamespaceDeclarationSyntax syn:
                {
                    syn.ContainingNamespaceSymbol = containingSymbol;
                    // TODO correctly handle Global qualifier
                    var sym = BuildNamespaceSymbol(containingSymbol, syn.DeclaredNames);
                    syn.Symbol.Fulfill(sym);
                    WalkChildren(syn, sym);
                }
                break;
                case INonMemberEntityDeclarationSyntax syn:
                    syn.ContainingNamespaceSymbol = containingSymbol;
                    break;
                default:
                    // do nothing
                    return;
            }
        }

        private NamespaceOrPackageSymbol BuildNamespaceSymbol(
            NamespaceOrPackageSymbol containingSymbol,
            NamespaceName namespaces)
        {
            foreach (var nsName in namespaces.Segments)
            {
                var nsSymbol = treeBuilder.Children(containingSymbol)
                                     .OfType<NamespaceSymbol>()
                                     .SingleOrDefault(c => c.Name == nsName);
                if (nsSymbol is null)
                {
                    nsSymbol = new NamespaceSymbol(containingSymbol, nsName);
                    treeBuilder.Add(nsSymbol);
                }

                containingSymbol = nsSymbol;
            }

            return containingSymbol;
        }
    }
}
