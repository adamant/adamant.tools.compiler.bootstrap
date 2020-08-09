using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    internal class SyntaxScopesBuilder : SyntaxWalker<Scope>
    {
        private readonly CodeFile file;
        private readonly GlobalScope globalScope;
        private readonly FixedList<Namespace> namespaces;
        private readonly Diagnostics diagnostics;

        public SyntaxScopesBuilder(
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

        protected override void WalkNonNull(ISyntax syntax, Scope containingScope)
        {
            switch (syntax)
            {
                case ICompilationUnitSyntax compilationUnit:
                    containingScope = BuildNamespaceScopes(compilationUnit.ImplicitNamespaceName.ToRootName(), containingScope);
                    containingScope = BuildUsingDirectivesScope(compilationUnit.UsingDirectives, containingScope);
                    break;
                case INamespaceDeclarationSyntax ns:
                    if (ns.IsGlobalQualified) containingScope = globalScope;
                    containingScope = BuildNamespaceScopes(ns.FullName.ToRootName(), containingScope);
                    containingScope = BuildUsingDirectivesScope(ns.UsingDirectives, containingScope);
                    break;
                case IClassDeclarationSyntax _:
                    // TODO name scope for type declaration
                    break;
                case IFunctionDeclarationSyntax function:
                    foreach (var parameter in function.Parameters) Walk(parameter, containingScope);
                    Walk(function.ReturnType, containingScope);
                    containingScope = BuildBodyScope(function.Parameters, containingScope);
                    Walk(function.Body, containingScope);
                    return;
                case IAssociatedFunctionDeclarationSyntax function:
                    foreach (var parameter in function.Parameters) Walk(parameter, containingScope);
                    Walk(function.ReturnType, containingScope);
                    containingScope = BuildBodyScope(function.Parameters, containingScope);
                    Walk(function.Body, containingScope);
                    return;
                case IConcreteMethodDeclarationSyntax concreteMethod:
                    Walk(concreteMethod.SelfParameter, containingScope);
                    foreach (var parameter in concreteMethod.Parameters) Walk(parameter, containingScope);
                    Walk(concreteMethod.ReturnType, containingScope);
                    containingScope = BuildBodyScope(concreteMethod.SelfParameter, concreteMethod.Parameters, containingScope);
                    Walk(concreteMethod.Body, containingScope);
                    return;
                case IConstructorDeclarationSyntax constructor:
                    Walk(constructor.ImplicitSelfParameter, containingScope);
                    foreach (var parameter in constructor.Parameters) Walk(parameter, containingScope);
                    containingScope = BuildBodyScope(constructor.ImplicitSelfParameter, constructor.Parameters, containingScope);
                    Walk(constructor.Body, containingScope);
                    return;
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
                case IHasContainingScope hasContainingScope:
                    hasContainingScope.ContainingScope = containingScope;
                    break;
            }

            WalkChildren(syntax, containingScope);
        }

        private Scope BuildNamespaceScopes(RootName nsName, Scope containingScope)
        {
            MaybeQualifiedName name;
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
            return new NestedScope(containingScope, @namespace.ChildMetadata, @namespace.NestedSymbols);
        }

        private Scope BuildUsingDirectivesScope(
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            Scope containingScope)
        {
            if (!usingDirectives.Any()) return containingScope;

            var importedSymbols = Enumerable.Empty<IMetadata>();
            foreach (var usingDirective in usingDirectives)
            {
                var @namespace = namespaces.SingleOrDefault(ns => ns.FullName.Equals(usingDirective.Name.ToRootName()));
                if (@namespace is null)
                {
                    diagnostics.Add(NameBindingError.UsingNonExistentNamespace(file, usingDirective.Span, (MaybeQualifiedName)usingDirective.Name.ToRootName()));
                    continue;
                }

                importedSymbols = importedSymbols.Concat(@namespace.ChildMetadata);
            }

            return new NestedScope(containingScope, importedSymbols);
        }

        private static Scope BuildBodyScope(
            IEnumerable<IParameterSyntax> parameters,
            Scope containingScope)
        {
            return new NestedScope(containingScope, parameters);
        }

        private static Scope BuildBodyScope(
            ISelfParameterSyntax selfParameter,
            IEnumerable<IParameterSyntax> parameters,
            Scope containingScope)
        {
            return new NestedScope(containingScope, parameters.Prepend(selfParameter));
        }
    }
}
