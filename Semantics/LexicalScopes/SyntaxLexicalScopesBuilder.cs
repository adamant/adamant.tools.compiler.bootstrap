using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class SyntaxLexicalScopesBuilder : SyntaxWalker<LexicalScope>
    {
        private readonly FixedList<ISymbol> nonMemberEntitySymbols;
        private readonly GlobalScope globalScope;

        public SyntaxLexicalScopesBuilder(FixedList<ISymbol> nonMemberEntitySymbols, GlobalScope globalScope)
        {
            this.nonMemberEntitySymbols = nonMemberEntitySymbols;
            this.globalScope = globalScope;
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
                case IConcreteMethodDeclarationSyntax concreteMethod:
                    foreach (var parameter in concreteMethod.Parameters) Walk(parameter, containingScope);
                    Walk(concreteMethod.ReturnTypeSyntax, containingScope);
                    containingScope = BuildBodyScope(concreteMethod.Parameters, containingScope);
                    Walk(concreteMethod.Body, containingScope);
                    return;
                case IConstructorDeclarationSyntax constructor:
                    foreach (var parameter in constructor.Parameters) Walk(parameter, containingScope);
                    containingScope = BuildBodyScope(constructor.Parameters, containingScope);
                    Walk(constructor.Body, containingScope);
                    return;
                case IHasContainingScope hasContainingScope:
                    hasContainingScope.ContainingScope = containingScope;
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    Walk(foreachExpression.TypeSyntax, containingScope);
                    Walk(foreachExpression.InExpression, containingScope);
                    containingScope = new NestedScope(containingScope, foreachExpression.Yield(), Enumerable.Empty<ISymbol>());
                    Walk(foreachExpression.Block, containingScope);
                    return;
                case IBodyOrBlockSyntax bodyOrBlock:
                    foreach (var statement in bodyOrBlock.Statements)
                    {
                        Walk(statement, containingScope);
                        // Each variable declaration effectively starts a new scope after it, this
                        // ensures a lookup returns the last declaration
                        if (statement is IVariableDeclarationStatementSyntax variableDeclaration)
                            containingScope = new NestedScope(
                                                containingScope,
                                                variableDeclaration.Yield(),
                                                Enumerable.Empty<ISymbol>());
                    }
                    return;
            }

            WalkChildren(syntax, containingScope);
        }

        private LexicalScope BuildNamespaceScopes(RootName ns, LexicalScope containingScope)
        {
            Name name;
            switch (ns)
            {
                default:
                    throw ExhaustiveMatch.Failed(ns);
                case GlobalNamespaceName _:
                    // TODO is this correct for namespace declarations that are global qualified (i.e. `::.`)?
                    // Perhaps it is because it could be nested inside other using statements
                    return containingScope;
                case QualifiedName qualifiedName:
                    containingScope = BuildNamespaceScopes(qualifiedName.Qualifier, containingScope);
                    name = qualifiedName;
                    break;
                case SimpleName simpleName:
                    name = simpleName;
                    break;
            }

            var symbolsInNamespace = nonMemberEntitySymbols.Where(s => s.FullName.HasQualifier(name));
            var symbolsNestedInNamespace = nonMemberEntitySymbols.Where(s => s.FullName.IsNestedIn(name));
            return new NestedScope(containingScope, symbolsInNamespace, symbolsNestedInNamespace);
        }

        private LexicalScope BuildUsingDirectivesScope(
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            LexicalScope containingScope)
        {
            if (!usingDirectives.Any()) return containingScope;

            var importedSymbols =
                usingDirectives.SelectMany(d => nonMemberEntitySymbols.Where(s => s.FullName.HasQualifier(d.Name)));
            return new NestedScope(containingScope, importedSymbols, Enumerable.Empty<ISymbol>());
        }

        private static LexicalScope BuildBodyScope(IEnumerable<IParameterSyntax> parameters, LexicalScope containingScope)
        {
            return new NestedScope(containingScope, parameters, Enumerable.Empty<ISymbol>());
        }
    }
}
