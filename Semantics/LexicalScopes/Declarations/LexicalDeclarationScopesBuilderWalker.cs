using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes.Declarations
{
    internal class LexicalDeclarationScopesBuilderWalker : SyntaxWalker<LexicalScope>
    {
        private readonly NestedScope globalScope;
        private readonly FixedDictionary<NamespaceName, Namespace> namespaces;

        public LexicalDeclarationScopesBuilderWalker(
            NestedScope globalScope,
            FixedDictionary<NamespaceName, Namespace> namespaces)
        {
            this.globalScope = globalScope;
            this.namespaces = namespaces;
        }

        protected override void WalkNonNull(ISyntax syntax, LexicalScope containingScope)
        {
            switch (syntax)
            {
                case ICompilationUnitSyntax syn:
                    containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.ImplicitNamespaceName, containingScope);
                    containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                    break;
                case INamespaceDeclarationSyntax syn:
                    syn.ContainingLexicalScope = containingScope;
                    if (syn.IsGlobalQualified)
                    {
                        containingScope = globalScope;
                        containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.FullName, containingScope);
                    }
                    else
                        containingScope = BuildNamespaceScopes(syn.ContainingNamespaceName, syn.DeclaredNames, containingScope);

                    containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                    break;
                case IClassDeclarationSyntax syn:
                    syn.ContainingLexicalScope = containingScope;
                    containingScope = BuildClassScope(syn, containingScope);
                    break;
                case IHasContainingLexicalScope syn:
                    syn.ContainingLexicalScope = containingScope;
                    break;
                case IBodySyntax _:
                case IExpressionSyntax _:
                    // Skip and don't walk children
                    return;
            }

            WalkChildren(syntax, containingScope);
        }

        private LexicalScope BuildNamespaceScopes(
            NamespaceName containingNamespaceName,
            NamespaceName declaredNamespaceNames,
            LexicalScope containingScope)
        {
            foreach (var name in declaredNamespaceNames.NamespaceNames())
            {
                var fullNamespaceName = containingNamespaceName.Qualify(name);
                // Skip the global namespace because we already have the global lexical scopes
                if (fullNamespaceName == NamespaceName.Global) continue;
                containingScope = BuildNamespaceScope(fullNamespaceName, containingScope);
            }

            return containingScope;
        }

        private LexicalScope BuildNamespaceScope(NamespaceName nsName, LexicalScope containingScope)
        {
            var ns = namespaces[nsName];
            return NestedScope.Create(containingScope, ns.SymbolsInPackage, ns.NestedSymbolsInPackage);
        }

        private LexicalScope BuildUsingDirectivesScope(
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            LexicalScope containingScope)
        {
            if (!usingDirectives.Any()) return containingScope;

            var importedSymbols = new Dictionary<TypeName, HashSet<IPromise<Symbol>>>();
            foreach (var usingDirective in usingDirectives)
            {
                if (!namespaces.TryGetValue(usingDirective.Name, out var ns))
                {
                    // TODO diagnostics.Add(NameBindingError.UsingNonExistentNamespace(file, usingDirective.Span, usingDirective.Name));
                    continue;
                }

                foreach (var (name, additionalSymbols) in ns.Symbols)
                {
                    if (importedSymbols.TryGetValue(name, out var symbols))
                        symbols.AddRange(additionalSymbols);
                    else
                        importedSymbols.Add(name, additionalSymbols.ToHashSet());
                }
            }

            var symbolsInScope = importedSymbols.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet());
            return NestedScope.Create(containingScope, symbolsInScope);
        }

        private static LexicalScope BuildClassScope(
            IClassDeclarationSyntax @class,
            LexicalScope containingScope)
        {
            // Only "static" names are in scope. Other names must use `self.`
            var symbols = @class.Members.OfType<IAssociatedFunctionDeclarationSyntax>()
                                .GroupBy(m => m.Name, m => m.Symbol)
                                .ToFixedDictionary(e => (TypeName)e.Key, e => e.ToFixedSet<IPromise<Symbol>>());

            return NestedScope.Create(containingScope, symbols);
        }
    }
}
