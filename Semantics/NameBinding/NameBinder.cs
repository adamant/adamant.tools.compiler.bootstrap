using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using JetBrains.Annotations;
using Void = Adamant.Tools.Compiler.Bootstrap.Framework.Void;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NameBinding
{
    public class NameBinder : ExpressionVisitor<LexicalScope, Void>
    {
        // TODO do we need a list of all the namespaces for validating using statements?
        // Gather a list of all the namespaces for validating using statements
        // Also need to account for empty directories?

        [NotNull, ItemNotNull] private readonly FixedList<ISymbol> allSymbols;
        [NotNull] private readonly GlobalScope globalScope;

        public NameBinder(
            [NotNull] PackageSyntax packageSyntax,
            [NotNull] FixedDictionary<string, Package> references)
        {
            allSymbols = GetAllSymbols(packageSyntax, references);
            globalScope = new GlobalScope(GetAllGlobalSymbols());
        }

        [NotNull]
        private static FixedList<ISymbol> GetAllSymbols(
            [NotNull] PackageSyntax packageSyntax,
            [NotNull] FixedDictionary<string, Package> references)
        {
            return references.Values.NotNull()
                    .SelectMany(p => p.Declarations).NotNull().Cast<ISymbol>()
                    .Concat(packageSyntax.CompilationUnits.SelectMany(cu => cu.NotNull().AllNamespacedDeclarations))
                    .ToFixedList();
        }

        [NotNull, ItemNotNull]
        private IEnumerable<ISymbol> GetAllGlobalSymbols()
        {
            return allSymbols.Where(s => s.NotNull().IsGlobal())
                .Concat(PrimitiveSymbols.Instance);
        }

        public void BindNamesInPackage([NotNull] PackageSyntax package)
        {
            foreach (var compilationUnit in package.CompilationUnits)
                BindNamesInCompilationUnit(compilationUnit);
        }

        private void BindNamesInCompilationUnit([NotNull] CompilationUnitSyntax compilationUnit)
        {
            var containingScope = BuildNamespaceScopes(globalScope, compilationUnit.ImplicitNamespaceName);
            containingScope = BuildUsingDirectivesScope(containingScope, compilationUnit.UsingDirectives);
            foreach (var declaration in compilationUnit.Declarations)
                BindNamesInDeclaration(containingScope, declaration);
        }

        private void BindNamesInDeclaration(
            [NotNull] LexicalScope containingScope,
            [NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case NamespaceDeclarationSyntax ns:
                {
                    if (ns.InGlobalNamespace)
                        containingScope = globalScope;

                    containingScope = BuildNamespaceScopes(containingScope, ns.Name);
                    containingScope = BuildUsingDirectivesScope(containingScope, ns.UsingDirectives);
                    foreach (var nestedDeclaration in ns.Declarations)
                        BindNamesInDeclaration(containingScope, nestedDeclaration);
                }
                break;
                case NamedFunctionDeclarationSyntax function:
                    BindNamesInFunctionParameters(containingScope, function);
                    VisitExpression(function.ReturnTypeExpression, containingScope);
                    BindNamesInFunctionBody(containingScope, function);
                    break;
                case OperatorDeclarationSyntax operatorDeclaration:
                    BindNamesInFunctionParameters(containingScope, operatorDeclaration);
                    VisitExpression(operatorDeclaration.ReturnTypeExpression, containingScope);
                    BindNamesInFunctionBody(containingScope, operatorDeclaration);
                    break;
                case ConstructorDeclarationSyntax constructor:
                    BindNamesInFunctionParameters(containingScope, constructor);
                    BindNamesInFunctionBody(containingScope, constructor);
                    break;
                case InitializerDeclarationSyntax initializer:
                    BindNamesInFunctionParameters(containingScope, initializer);
                    BindNamesInFunctionBody(containingScope, initializer);
                    break;
                case TypeDeclarationSyntax typeDeclaration:
                    // TODO name scope for type declaration
                    foreach (var nestedDeclaration in typeDeclaration.Members)
                        BindNamesInDeclaration(containingScope, (DeclarationSyntax)nestedDeclaration);
                    break;
                case FieldDeclarationSyntax fieldDeclaration:
                    VisitExpression(fieldDeclaration.TypeExpression, containingScope);
                    VisitExpression(fieldDeclaration.Initializer, containingScope);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void BindNamesInFunctionParameters(
            [NotNull] LexicalScope containingScope,
            [NotNull] FunctionDeclarationSyntax function)
        {
            if (function.GenericParameters != null)
                foreach (var parameter in function.GenericParameters)
                    VisitExpression(parameter.TypeExpression, containingScope);

            foreach (var parameter in function.Parameters)
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                        VisitExpression(namedParameter.TypeExpression, containingScope);
                        break;
                    case SelfParameterSyntax _:
                    case FieldParameterSyntax _:
                        // Nothing to bind
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(parameter);
                }
        }

        private void BindNamesInFunctionBody(
            [NotNull] LexicalScope containingScope,
            [NotNull] FunctionDeclarationSyntax function)
        {
            var symbols = new List<ISymbol>();
            foreach (var parameter in function.Parameters)
                symbols.Add(parameter);

            containingScope = new NestedScope(containingScope, symbols);
            BindNamesInBlock(containingScope, function.Body);
        }

        private void BindNamesInBlock([NotNull] LexicalScope containingScope, [CanBeNull] BlockSyntax block)
        {
            if (block == null) return;

            var symbols = new List<ISymbol>();

            foreach (var variableDeclaration in block.Statements
                .OfType<VariableDeclarationStatementSyntax>())
                symbols.Add(variableDeclaration);

            containingScope = new NestedScope(containingScope, symbols);

            foreach (var statement in block.Statements)
                VisitStatement(statement, containingScope);
        }

        public override Void VisitExpression([CanBeNull] ExpressionSyntax expression, LexicalScope containingScope)
        {
            return expression == null ? default : base.VisitExpression(expression, containingScope);
        }

        public override Void VisitIdentifierName(
            [NotNull] IdentifierNameSyntax identifierName,
            [NotNull] LexicalScope containingScope)
        {
            // TODO if the name wasn't found, put in some default, unknown symbol
            identifierName.ReferencedSymbol = containingScope.Lookup(identifierName.Name);
            return default;
        }

        [NotNull]
        private LexicalScope BuildNamespaceScopes(
            [NotNull] LexicalScope containingScope,
            [NotNull] RootName ns)
        {
            Name name;
            switch (ns)
            {
                case GlobalNamespaceName _:
                    return containingScope;
                case QualifiedName qualifiedName:
                    containingScope = BuildNamespaceScopes(containingScope, qualifiedName.Qualifier);
                    name = qualifiedName;
                    break;
                case SimpleName simpleName:
                    name = simpleName;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(ns);
            }

            var symbolsInNamespace = allSymbols.Where(s => s.FullName.HasQualifier(name));
            return new NestedScope(containingScope, symbolsInNamespace);
        }

        [NotNull]
        private LexicalScope BuildUsingDirectivesScope(
            [NotNull] LexicalScope containingScope,
            [NotNull, ItemNotNull] FixedList<UsingDirectiveSyntax> usingDirectives)
        {
            if (!usingDirectives.Any()) return containingScope;

            var importedSymbols = usingDirectives
                .SelectMany(d => allSymbols.Where(s => s.FullName.HasQualifier(d.Name)));
            return new NestedScope(containingScope, importedSymbols);
        }
    }
}
