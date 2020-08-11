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
    internal class LexicalScopesBuilderWalker : SyntaxWalker<LexicalScope>
    {
        private readonly NestedScope globalScope;
        private readonly FixedDictionary<NamespaceName, Namespace> namespaces;

        public LexicalScopesBuilderWalker(
            NestedScope globalScope,
            FixedDictionary<NamespaceName, Namespace> namespaces)
        {
            this.globalScope = globalScope;
            this.namespaces = namespaces;
        }

        protected override void WalkNonNull(ISyntax syntax, LexicalScope containingScope)
        {
            if (syntax is IHasContainingLexicalScope hasContainingLexicalScope)
                hasContainingLexicalScope.ContainingLexicalScope = containingScope;

            switch (syntax)
            {
                case ICompilationUnitSyntax syn:
                    containingScope = BuildNamespaceScopes(NamespaceName.Global, syn.ImplicitNamespaceName, containingScope);
                    containingScope = BuildUsingDirectivesScope(syn.UsingDirectives, containingScope);
                    break;
                case INamespaceDeclarationSyntax syn:
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
                    containingScope = BuildClassScope(syn, containingScope);
                    break;
                case IFunctionDeclarationSyntax function:
                    foreach (var parameter in function.Parameters)
                        Walk(parameter, containingScope);
                    Walk(function.ReturnType, containingScope);
                    containingScope = BuildBodyScope(function.Parameters, containingScope);
                    Walk(function.Body, containingScope);
                    return;
                case IAssociatedFunctionDeclarationSyntax function:
                    foreach (var parameter in function.Parameters)
                        Walk(parameter, containingScope);
                    Walk(function.ReturnType, containingScope);
                    containingScope = BuildBodyScope(function.Parameters, containingScope);
                    Walk(function.Body, containingScope);
                    return;
                case IConcreteMethodDeclarationSyntax concreteMethod:
                    Walk(concreteMethod.SelfParameter, containingScope);
                    foreach (var parameter in concreteMethod.Parameters)
                        Walk(parameter, containingScope);
                    Walk(concreteMethod.ReturnType, containingScope);
                    containingScope = BuildBodyScope(concreteMethod.Parameters, containingScope);
                    Walk(concreteMethod.Body, containingScope);
                    return;
                case IConstructorDeclarationSyntax constructor:
                    Walk(constructor.ImplicitSelfParameter, containingScope);
                    foreach (var parameter in constructor.Parameters)
                        Walk(parameter, containingScope);
                    containingScope = BuildBodyScope(constructor.Parameters, containingScope);
                    Walk(constructor.Body, containingScope);
                    return;
                case IBodyOrBlockSyntax bodyOrBlock:
                    foreach (var statement in bodyOrBlock.Statements)
                    {
                        Walk(statement, containingScope);
                        // Each variable declaration effectively starts a new scope after it, this
                        // ensures a lookup returns the last declaration
                        if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                            containingScope = BuildVariableScope(containingScope, variableDeclaration.Name, variableDeclaration.Symbol);
                    }
                    return;
                case IForeachExpressionSyntax foreachExpression:
                    Walk(foreachExpression.Type, containingScope);
                    Walk(foreachExpression.InExpression, containingScope);
                    containingScope = BuildVariableScope(containingScope, foreachExpression.VariableName, foreachExpression.Symbol);
                    Walk(foreachExpression.Block, containingScope);
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

        private static LexicalScope BuildBodyScope(
            IEnumerable<IConstructorParameterSyntax> parameters,
            LexicalScope containingScope)
        {
            var symbols = parameters.GroupBy(p => p.Name, p => p.Symbol)
                                    .ToFixedDictionary(e => (TypeName)e.Key, e => e.ToFixedSet<IPromise<Symbol>>());
            return NestedScope.Create(containingScope, symbols);
        }

        private static LexicalScope BuildVariableScope(
            LexicalScope containingScope,
            Name name,
            Promise<VariableSymbol> symbol)
        {
            var symbols = new Dictionary<TypeName, FixedSet<IPromise<Symbol>>>()
            {
                { name, symbol.Yield().ToFixedSet<IPromise<Symbol>>() }
            }.ToFixedDictionary();
            return NestedScope.Create(containingScope, symbols);
        }
    }
}
