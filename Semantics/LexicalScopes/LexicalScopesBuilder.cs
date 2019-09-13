using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
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
            var binder = new ExpressionLexicalScopesBuilder();
            var diagnosticCount = diagnostics.Count;
            BuildScopesInDeclaration(declaration, containingScope, binder);
            if (diagnosticCount != diagnostics.Count)
                declaration.MarkErrored();
        }

        private void BuildScopesInDeclaration(
            DeclarationSyntax declaration,
            LexicalScope containingScope,
            ExpressionLexicalScopesBuilder binder)
        {
            switch (declaration)
            {
                case NamespaceDeclarationSyntax ns:
                    if (ns.InGlobalNamespace)
                        containingScope = globalScope;

                    containingScope = BuildNamespaceScopes(ns.Name, containingScope);
                    containingScope =
                        BuildUsingDirectivesScope(ns.UsingDirectives, containingScope);
                    foreach (var nestedDeclaration in ns.Declarations)
                        BuildScopesInDeclaration(nestedDeclaration, containingScope);
                    break;
                case NamedFunctionDeclarationSyntax function:
                    BuildScopesInFunctionParameters(function, containingScope, binder);
                    binder.VisitExpression(function.ReturnTypeExpression, containingScope);
                    BuildScopesInFunctionBody(function, containingScope, binder);
                    break;
                case OperatorDeclarationSyntax operatorDeclaration:
                    BuildScopesInFunctionParameters(operatorDeclaration, containingScope, binder);
                    binder.VisitExpression(operatorDeclaration.ReturnTypeExpression, containingScope);
                    BuildScopesInFunctionBody(operatorDeclaration, containingScope, binder);
                    break;
                case ConstructorDeclarationSyntax constructor:
                    BuildScopesInFunctionParameters(constructor, containingScope, binder);
                    BuildScopesInFunctionBody(constructor, containingScope, binder);
                    break;
                case InitializerDeclarationSyntax initializer:
                    BuildScopesInFunctionParameters(initializer, containingScope, binder);
                    BuildScopesInFunctionBody(initializer, containingScope, binder);
                    break;
                case TypeDeclarationSyntax typeDeclaration:
                    // TODO name scope for type declaration
                    foreach (var nestedDeclaration in typeDeclaration.Members)
                        BuildScopesInDeclaration(nestedDeclaration, containingScope);
                    break;
                case FieldDeclarationSyntax fieldDeclaration:
                    binder.VisitExpression(fieldDeclaration.TypeExpression, containingScope);
                    binder.VisitExpression(fieldDeclaration.Initializer, containingScope);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private static void BuildScopesInFunctionParameters(
            FunctionDeclarationSyntax function,
            LexicalScope containingScope,
            ExpressionLexicalScopesBuilder binder)
        {
            if (function.GenericParameters != null)
                foreach (var parameter in function.GenericParameters)
                    binder.VisitExpression(parameter.TypeExpression, containingScope);

            foreach (var parameter in function.Parameters)
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                        binder.VisitExpression(namedParameter.TypeExpression, containingScope);
                        break;
                    case SelfParameterSyntax _:
                    case FieldParameterSyntax _:
                        // Nothing to bind
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(parameter);
                }
        }

        private static void BuildScopesInFunctionBody(
            FunctionDeclarationSyntax function,
            LexicalScope containingScope,
            ExpressionLexicalScopesBuilder binder)
        {
            var symbols = new List<ISymbol>();
            foreach (var parameter in function.Parameters)
                symbols.Add(parameter);

            containingScope = new NestedScope(containingScope, symbols, Enumerable.Empty<ISymbol>());
            binder.VisitBlock(function.Body, containingScope);
        }

        private LexicalScope BuildNamespaceScopes(
            RootName ns,
            LexicalScope containingScope)
        {
            Name name;
            switch (ns)
            {
                case GlobalNamespaceName _:
                    return containingScope;
                case QualifiedName qualifiedName:
                    containingScope = BuildNamespaceScopes(qualifiedName.Qualifier, containingScope);
                    name = qualifiedName;
                    break;
                case SimpleName simpleName:
                    name = simpleName;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(ns);
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
