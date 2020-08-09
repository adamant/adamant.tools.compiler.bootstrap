using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class DeclarationLexicalScopesBuilderWalker : SyntaxWalker<LexicalScope<IPromise<Symbol>>>
    {
        private readonly FixedDictionary<NamespaceName, Namespace> namespaces;

        public DeclarationLexicalScopesBuilderWalker(FixedDictionary<NamespaceName, Namespace> namespaces)
        {
            this.namespaces = namespaces;
        }

        protected override void WalkNonNull(ISyntax syntax, LexicalScope<IPromise<Symbol>> containingScope)
        {
            switch (syntax)
            {
                case ICompilationUnitSyntax syn:
                    containingScope = BuildNamespaceScopes(syn.ImplicitNamespaceName, containingScope);
                    containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                    WalkChildren(syn, containingScope);
                    break;
                case INamespaceDeclarationSyntax syn:
                    syn.ContainingLexicalScope = containingScope;
                    // TODO BuildNamespaceScopes
                    containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                    WalkChildren(syn, containingScope);
                    break;
                case INonMemberDeclarationSyntax syn:
                    syn.ContainingLexicalScope = containingScope;
                    break;
            }
        }

        private LexicalScope<IPromise<Symbol>> BuildNamespaceScopes(NamespaceName nsName, LexicalScope<IPromise<Symbol>> containingScope)
        {
            foreach (var name in nsName.NamespaceNames())
                containingScope = BuildNamespaceScope(name, containingScope);

            return containingScope;
        }

        private LexicalScope<IPromise<Symbol>> BuildNamespaceScope(NamespaceName nsName, LexicalScope<IPromise<Symbol>> containingScope)
        {
            var ns = namespaces[nsName];
            return new NestedScope<IPromise<Symbol>>(containingScope, ns.Symbols, ns.NestedSymbols);
        }

        private LexicalScope<IPromise<Symbol>> BuildUsingDirectivesScope(
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            LexicalScope<IPromise<Symbol>> containingScope)
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
            return new NestedScope<IPromise<Symbol>>(containingScope, symbolsInScope);
        }
    }
}
