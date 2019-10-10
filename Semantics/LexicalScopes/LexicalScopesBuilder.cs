using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    /// <summary>
    /// Builds up a hierarchy of lexical scope objects that are later used to look
    /// up names for name binding
    /// </summary>
    public class LexicalScopesBuilder
    {
        // TODO do we need a list of all the namespaces for validating using statements?
        // Gather a list of all the namespaces for validating using statements
        // Also need to account for empty directories?

        private readonly Diagnostics diagnostics;
        private readonly FixedList<ISymbol> nonMemberEntitySymbols;
        private readonly GlobalScope globalScope;

        public LexicalScopesBuilder(
             Diagnostics diagnostics,
             PackageSyntax packageSyntax,
             FixedDictionary<string, Package> references)
        {
            this.diagnostics = diagnostics;
            nonMemberEntitySymbols = GetNonMemberEntitySymbols(packageSyntax, references);
            globalScope = new GlobalScope(GetAllGlobalSymbols(nonMemberEntitySymbols), nonMemberEntitySymbols);
        }

        private static FixedList<ISymbol> GetNonMemberEntitySymbols(
             PackageSyntax packageSyntax,
             FixedDictionary<string, Package> references)
        {
            return references.Values
                .SelectMany(p => p.Declarations.Where(d => !d.IsMember))
                .Concat(packageSyntax.CompilationUnits.SelectMany(GetAllNonMemberDeclarations))
                .ToFixedList();
        }

        /// <summary>
        /// This gets the symbols for all entities that are declared outside of a class.
        /// (i.e. directly in a namespace)
        /// </summary>
        private static FixedList<ISymbol> GetAllNonMemberDeclarations(ICompilationUnitSyntax compilationUnit)
        {
            var declarations = new List<ISymbol>();
            declarations.AddRange(compilationUnit.Declarations.OfType<INonMemberEntityDeclarationSyntax>());
            var namespaces = new Queue<INamespaceDeclarationSyntax>();
            namespaces.EnqueueRange(compilationUnit.Declarations.OfType<INamespaceDeclarationSyntax>());
            while (namespaces.TryDequeue(out var ns))
            {
                declarations.AddRange(ns.Declarations.OfType<INonMemberEntityDeclarationSyntax>());
                namespaces.EnqueueRange(ns.Declarations.OfType<INamespaceDeclarationSyntax>());
            }

            return declarations.ToFixedList();
        }

        private static IEnumerable<ISymbol> GetAllGlobalSymbols(FixedList<ISymbol> symbols)
        {
            return symbols.Where(s => s.IsGlobal())
                .Concat(PrimitiveSymbols.Instance);
        }

        public void BuildScopesInPackage(PackageSyntax package)
        {
            foreach (var compilationUnit in package.CompilationUnits)
                BuildScopesInCompilationUnit(compilationUnit);
        }

        private void BuildScopesInCompilationUnit(ICompilationUnitSyntax compilationUnit)
        {
            var containingScope = BuildNamespaceScopes(compilationUnit.ImplicitNamespaceName, globalScope);
            containingScope = BuildUsingDirectivesScope(compilationUnit.UsingDirectives, containingScope);
            foreach (var declaration in compilationUnit.Declarations)
                BuildScopesInDeclaration(declaration, containingScope);
        }

        private void BuildScopesInDeclaration(
            INonMemberDeclarationSyntax declaration,
            LexicalScope containingScope)
        {
            var binder = new ExpressionScopesBuilder();
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case INamespaceDeclarationSyntax ns:
                    if (ns.IsGlobalQualified)
                        containingScope = globalScope;

                    containingScope = BuildNamespaceScopes(ns.Name, containingScope);
                    containingScope = BuildUsingDirectivesScope(ns.UsingDirectives, containingScope);
                    foreach (var nestedDeclaration in ns.Declarations)
                        BuildScopesInDeclaration(nestedDeclaration, containingScope);
                    break;
                case IFunctionDeclarationSyntax function:
                    BuildScopesInFunctionParameters(containingScope, function.Parameters);
                    if (function.ReturnTypeSyntax != null)
                        new TypeScopesBuilder(containingScope).Walk(function.ReturnTypeSyntax);
                    BuildScopesInBody(containingScope, binder, function.Parameters, function.Body);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    // TODO name scope for type declaration
                    foreach (var nestedDeclaration in classDeclaration.Members)
                        BuildScopesInDeclaration(nestedDeclaration, containingScope);
                    break;
            }
        }

        private void BuildScopesInDeclaration(
            IMemberDeclarationSyntax declaration,
            LexicalScope containingScope)
        {
            var binder = new ExpressionScopesBuilder();
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IConcreteMethodDeclarationSyntax concreteMethod:
                    BuildScopesInFunctionParameters(containingScope, concreteMethod.Parameters);
                    if (concreteMethod.ReturnTypeSyntax != null)
                        new TypeScopesBuilder(containingScope).Walk(concreteMethod.ReturnTypeSyntax);
                    BuildScopesInBody(containingScope, binder, concreteMethod.Parameters, concreteMethod.Body);
                    break;
                case IAbstractMethodDeclarationSyntax abstractMethod:
                    BuildScopesInFunctionParameters(containingScope, abstractMethod.Parameters);
                    if (abstractMethod.ReturnTypeSyntax != null)
                        new TypeScopesBuilder(containingScope).Walk(abstractMethod.ReturnTypeSyntax);
                    break;
                case IConstructorDeclarationSyntax constructor:
                    BuildScopesInFunctionParameters(containingScope, constructor.Parameters);
                    BuildScopesInBody(containingScope, binder, constructor.Parameters, constructor.Body);
                    break;
                case IFieldDeclarationSyntax fieldDeclaration:
                    new TypeScopesBuilder(containingScope).Walk(fieldDeclaration.TypeSyntax);
                    binder.VisitExpression(fieldDeclaration.Initializer, containingScope);
                    break;
            }
        }

        private static void BuildScopesInFunctionParameters(
            LexicalScope containingScope,
            FixedList<IParameterSyntax> parameters)
        {
            foreach (var parameter in parameters)
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case INamedParameterSyntax namedParameter:
                        new TypeScopesBuilder(containingScope).Walk(namedParameter.TypeSyntax);
                        break;
                    case ISelfParameterSyntax _:
                    case IFieldParameterSyntax _:
                        // Nothing to bind
                        break;
                }
        }

        private static void BuildScopesInBody(
            LexicalScope containingScope,
            ExpressionScopesBuilder binder,
            FixedList<IParameterSyntax> parameters,
            FixedList<IStatementSyntax> body)
        {
            var symbols = new List<ISymbol>();
            foreach (var parameter in parameters)
                symbols.Add(parameter);

            containingScope = new NestedScope(containingScope, symbols, Enumerable.Empty<ISymbol>());
            binder.VisitStatements(body, containingScope);
        }

        private LexicalScope BuildNamespaceScopes(
            RootName ns,
            LexicalScope containingScope)
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
            if (!usingDirectives.Any())
                return containingScope;

            var importedSymbols = usingDirectives
                .SelectMany(d => nonMemberEntitySymbols.Where(s => s.FullName.HasQualifier(d.Name)));
            return new NestedScope(containingScope, importedSymbols, Enumerable.Empty<ISymbol>());
        }
    }
}
