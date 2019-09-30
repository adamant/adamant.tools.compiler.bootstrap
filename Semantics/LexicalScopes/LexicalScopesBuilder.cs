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
        private readonly FixedList<ISymbol> allSymbols;
        private readonly GlobalScope globalScope;

        public LexicalScopesBuilder(
             Diagnostics diagnostics,
             PackageSyntax packageSyntax,
             FixedDictionary<string, Package> references)
        {
            this.diagnostics = diagnostics;
            allSymbols = GetAllSymbols(packageSyntax, references);
            globalScope = new GlobalScope(GetAllGlobalSymbols(), allSymbols);
        }

        private static FixedList<ISymbol> GetAllSymbols(
             PackageSyntax packageSyntax,
             FixedDictionary<string, Package> references)
        {
            return references.Values
                .SelectMany(p => p.Declarations.Where(d => !d.IsMember))
                .Concat(packageSyntax.CompilationUnits.SelectMany(GetAllNonMemberDeclarations))
                .ToFixedList();
        }

        /// <summary>
        /// This gets the symbols for all declarations that are declared outside of a type.
        /// (i.e. directly in a namespace)
        /// </summary>
        private static FixedList<ISymbol> GetAllNonMemberDeclarations(CompilationUnitSyntax compilationUnit)
        {
            // MemberDeclarationSyntax is the right type to use here because anything except namespaces can go in a type
            var declarations = new List<ISymbol>();
            declarations.AddRange(compilationUnit.Declarations.OfType<MemberDeclarationSyntax>());
            var namespaces = new Queue<NamespaceDeclarationSyntax>();
            namespaces.EnqueueRange(compilationUnit.Declarations.OfType<NamespaceDeclarationSyntax>());
            while (namespaces.TryDequeue(out var ns))
            {
                declarations.AddRange(ns.Declarations.OfType<MemberDeclarationSyntax>());
                namespaces.EnqueueRange(ns.Declarations.OfType<NamespaceDeclarationSyntax>());
            }

            return declarations.ToFixedList();
        }

        private IEnumerable<ISymbol> GetAllGlobalSymbols()
        {
            return allSymbols.Where(s => s.IsGlobal())
                .Concat(PrimitiveSymbols.Instance);
        }

        public void BuildScopesInPackage(PackageSyntax package)
        {
            foreach (var compilationUnit in package.CompilationUnits)
                BuildScopesInCompilationUnit(compilationUnit);
        }

        private void BuildScopesInCompilationUnit(CompilationUnitSyntax compilationUnit)
        {
            var containingScope = BuildNamespaceScopes(compilationUnit.ImplicitNamespaceName, globalScope);
            containingScope = BuildUsingDirectivesScope(compilationUnit.UsingDirectives, containingScope);
            foreach (var declaration in compilationUnit.Declarations)
                BuildScopesInDeclaration(declaration, containingScope);
        }

        private void BuildScopesInDeclaration(
            DeclarationSyntax declaration,
            LexicalScope containingScope)
        {
            var binder = new ExpressionScopesBuilder();
            var diagnosticCount = diagnostics.Count;
            BuildScopesInDeclaration(declaration, containingScope, binder);
            if (diagnosticCount != diagnostics.Count)
                declaration.MarkErrored();
        }

        private void BuildScopesInDeclaration(
            IDeclarationSyntax declaration,
            LexicalScope containingScope,
            ExpressionScopesBuilder binder)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case INamespaceDeclarationSyntax ns:
                    if (ns.InGlobalNamespace)
                        containingScope = globalScope;

                    containingScope = BuildNamespaceScopes(ns.Name, containingScope);
                    containingScope =
                        BuildUsingDirectivesScope(ns.UsingDirectives, containingScope);
                    foreach (var nestedDeclaration in ns.Declarations)
                        BuildScopesInDeclaration(nestedDeclaration, containingScope);
                    break;
                case INamedFunctionDeclarationSyntax function:
                    BuildScopesInFunctionParameters(function, containingScope, binder);
                    if (function.ReturnTypeSyntax != null)
                        new TypeScopesBuilder(containingScope).Walk(function.ReturnTypeSyntax);
                    BuildScopesInFunctionBody(function, containingScope, binder);
                    break;
                case IConstructorDeclarationSyntax constructor:
                    BuildScopesInFunctionParameters(constructor, containingScope, binder);
                    BuildScopesInFunctionBody(constructor, containingScope, binder);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    // TODO name scope for type declaration
                    foreach (var nestedDeclaration in classDeclaration.Members)
                        BuildScopesInDeclaration(nestedDeclaration, containingScope);
                    break;
                case IFieldDeclarationSyntax fieldDeclaration:
                    new TypeScopesBuilder(containingScope).Walk(fieldDeclaration.TypeSyntax);
                    binder.VisitExpression(fieldDeclaration.Initializer, containingScope);
                    break;
            }
        }

        private static void BuildScopesInFunctionParameters(
            IFunctionDeclarationSyntax function,
            LexicalScope containingScope,
            ExpressionScopesBuilder binder)
        {
            foreach (var parameter in function.Parameters)
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case NamedParameterSyntax namedParameter:
                        new TypeScopesBuilder(containingScope).Walk(namedParameter.TypeSyntax);
                        break;
                    case SelfParameterSyntax _:
                    case FieldParameterSyntax _:
                        // Nothing to bind
                        break;
                }
        }

        private static void BuildScopesInFunctionBody(
            IFunctionDeclarationSyntax function,
            LexicalScope containingScope,
            ExpressionScopesBuilder binder)
        {
            var symbols = new List<ISymbol>();
            foreach (var parameter in function.Parameters)
                symbols.Add(parameter);

            containingScope = new NestedScope(containingScope, symbols, Enumerable.Empty<ISymbol>());
            foreach (var statement in function.Body)
                binder.VisitStatement(statement, containingScope);
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
                    return containingScope;
                case QualifiedName qualifiedName:
                    containingScope = BuildNamespaceScopes(qualifiedName.Qualifier, containingScope);
                    name = qualifiedName;
                    break;
                case SimpleName simpleName:
                    name = simpleName;
                    break;
            }

            var symbolsInNamespace = allSymbols.Where(s => s.FullName.HasQualifier(name));
            var symbolsNestedInNamespace = allSymbols.Where(s => s.FullName.IsNestedIn(name));
            return new NestedScope(containingScope, symbolsInNamespace, symbolsNestedInNamespace);
        }

        private LexicalScope BuildUsingDirectivesScope(
            FixedList<UsingDirectiveSyntax> usingDirectives,
            LexicalScope containingScope)
        {
            if (!usingDirectives.Any())
                return containingScope;

            var importedSymbols = usingDirectives
                .SelectMany(d => allSymbols.Where(s => s.FullName.HasQualifier(d.Name)));
            return new NestedScope(containingScope, importedSymbols, Enumerable.Empty<ISymbol>());
        }
    }
}
