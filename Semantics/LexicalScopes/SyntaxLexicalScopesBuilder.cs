using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class SyntaxLexicalScopesBuilder : SyntaxWalker<LexicalScope>
    {
        private readonly CodeFile file;
        private readonly GlobalScope globalScope;
        private readonly FixedList<Namespace> namespaces;
        private readonly Diagnostics diagnostics;

        public SyntaxLexicalScopesBuilder(
            CodeFile file,
            GlobalScope globalScope,
            FixedList<Namespace> namespaces,
            Diagnostics diagnostics)
        {
            this.file = file;
            this.globalScope = globalScope;
            this.namespaces = namespaces;
            this.diagnostics = diagnostics;
        }

        protected override void WalkNonNull(ISyntax syntax, LexicalScope containingScope)
        {
            switch (syntax)
            {
                case ICompilationUnitSyntax compilationUnit:
                    containingScope = BuildNamespaceScopes(compilationUnit.ImplicitNamespaceName, containingScope);
                    containingScope = BuildUsingDirectivesScope(compilationUnit.UsingDirectives, containingScope);
                    break;
                case INamespaceDeclarationSyntax ns:
                    if (ns.IsGlobalQualified) containingScope = globalScope;
                    containingScope = BuildNamespaceScopes(ns.Name, containingScope);
                    containingScope = BuildUsingDirectivesScope(ns.UsingDirectives, containingScope);
                    break;
                case IClassDeclarationSyntax _:
                    // TODO name scope for type declaration
                    break;
                case IFunctionDeclarationSyntax function:
                    foreach (var parameter in function.Parameters) Walk(parameter, containingScope);
                    Walk(function.ReturnTypeSyntax, containingScope);
                    containingScope = BuildBodyScope(function.Parameters, containingScope);
                    Walk(function.Body, containingScope);
                    return;
                case IAssociatedFunctionDeclaration function:
                    foreach (var parameter in function.Parameters) Walk(parameter, containingScope);
                    Walk(function.ReturnTypeSyntax, containingScope);
                    containingScope = BuildBodyScope(function.Parameters, containingScope);
                    Walk(function.Body, containingScope);
                    return;
                case IConcreteMethodDeclarationSyntax concreteMethod:
                    Walk(concreteMethod.SelfParameter, containingScope);
                    foreach (var parameter in concreteMethod.Parameters) Walk(parameter, containingScope);
                    Walk(concreteMethod.ReturnTypeSyntax, containingScope);
                    containingScope = BuildBodyScope(concreteMethod.SelfParameter, concreteMethod.Parameters, containingScope);
                    Walk(concreteMethod.Body, containingScope);
                    return;
                case IConstructorDeclarationSyntax constructor:
                    Walk(constructor.ImplicitSelfParameter, containingScope);
                    foreach (var parameter in constructor.Parameters) Walk(parameter, containingScope);
                    containingScope = BuildBodyScope(constructor.ImplicitSelfParameter, constructor.Parameters, containingScope);
                    Walk(constructor.Body, containingScope);
                    return;
                case IHasContainingScope hasContainingScope:
                    hasContainingScope.ContainingScope = containingScope;
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    Walk(foreachExpression.TypeSyntax, containingScope);
                    Walk(foreachExpression.InExpression, containingScope);
                    containingScope = new NestedScope(containingScope, foreachExpression);
                    Walk(foreachExpression.Block, containingScope);
                    return;
                case IBodyOrBlockSyntax bodyOrBlock:
                    foreach (var statement in bodyOrBlock.Statements)
                    {
                        Walk(statement, containingScope);
                        // Each variable declaration effectively starts a new scope after it, this
                        // ensures a lookup returns the last declaration
                        if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                            containingScope = new NestedScope(containingScope, variableDeclaration);
                    }
                    return;
            }

            WalkChildren(syntax, containingScope);
        }

        private LexicalScope BuildNamespaceScopes(RootName nsName, LexicalScope containingScope)
        {
            Name name;
            switch (nsName)
            {
                default:
                    throw ExhaustiveMatch.Failed(nsName);
                case GlobalNamespaceName _:
                    // Compilation units can have the global namespace as their namespace name
                    return globalScope;
                case QualifiedName qualifiedName:
                    containingScope = BuildNamespaceScopes(qualifiedName.Qualifier, containingScope);
                    name = qualifiedName;
                    break;
                case SimpleName simpleName:
                    name = simpleName;
                    break;
            }

            var @namespace = namespaces.Single(ns => ns.FullName.Equals(name));
            return new NestedScope(containingScope, @namespace.ChildSymbols, @namespace.NestedSymbols);
        }

        private LexicalScope BuildUsingDirectivesScope(
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            LexicalScope containingScope)
        {
            if (!usingDirectives.Any()) return containingScope;

            var importedSymbols = Enumerable.Empty<ISymbol>();
            foreach (var usingDirective in usingDirectives)
            {
                var @namespace = namespaces.SingleOrDefault(ns => ns.FullName.Equals(usingDirective.Name));
                if (@namespace is null)
                {
                    diagnostics.Add(NameBindingError.UsingNonExistentNamespace(file, usingDirective.Span, usingDirective.Name));
                    continue;
                }

                importedSymbols = importedSymbols.Concat(@namespace.ChildSymbols);
            }

            return new NestedScope(containingScope, importedSymbols);
        }

        private static LexicalScope BuildBodyScope(
            IEnumerable<IParameterSyntax> parameters,
            LexicalScope containingScope)
        {
            return new NestedScope(containingScope, parameters);
        }

        private static LexicalScope BuildBodyScope(
            ISelfParameterSyntax selfParameter,
            IEnumerable<IParameterSyntax> parameters,
            LexicalScope containingScope)
        {
            return new NestedScope(containingScope, parameters.Prepend(selfParameter));
        }
    }
}
