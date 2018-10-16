using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class AnalysisBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;

        public AnalysisBuilder([NotNull] NameBuilder nameBuilder)
        {
            this.nameBuilder = nameBuilder;
        }

        public (IList<CompilationUnitScope>, IList<DeclarationAnalysis>) PrepareForAnalysis(
            [NotNull] PackageSyntax packageSyntax)
        {
            var scopes = new List<CompilationUnitScope>();
            var analyses = new List<DeclarationAnalysis>();
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                var globalScope = new CompilationUnitScope();
                scopes.Add(globalScope);
                analyses.AddRange(PrepareForAnalysis(compilationUnit, globalScope));
            }

            return (scopes, analyses);
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DeclarationAnalysis> PrepareForAnalysis(
            [NotNull] CompilationUnitSyntax compilationUnit,
            [NotNull] CompilationUnitScope globalScope)
        {
            Name @namespace = GlobalNamespaceName.Instance;
            LexicalScope scope = globalScope;
            if (compilationUnit.Namespace != null)
            {
                @namespace = nameBuilder.BuildName(compilationUnit.Namespace.Name) ?? @namespace;
                scope = new NamespaceScope(scope);
            }

            foreach (var declaration in compilationUnit.Declarations)
            {
                var analysis = PrepareForAnalysis(compilationUnit.CodeFile, scope, @namespace, declaration);
                if (analysis != null)
                    yield return analysis;
            }
        }

        [CanBeNull]
        private static DeclarationAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    return PrepareForAnalysis(codeFile, scope, @namespace, function);
                case ClassDeclarationSyntax @class:
                    return PrepareForAnalysis(codeFile, scope, @namespace, @class);
                case IncompleteDeclarationSyntax _:
                    // Since it is incomplete, we can't do any analysis on it
                    return null;
                case EnumStructDeclarationSyntax enumStruct:
                    return PrepareForAnalysis(codeFile, scope, @namespace, enumStruct);
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private static FunctionAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] FunctionDeclarationSyntax syntax)
        {
            // Skip any function that doesn't have a name
            if (!(syntax.Name?.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var semantics = new FunctionDeclaration(codeFile, fullName);
            return new FunctionAnalysis(codeFile, scope, syntax, semantics);
        }

        private static TypeAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] ClassDeclarationSyntax syntax)
        {
            // Skip any class that doesn't have a name
            if (!(syntax.Name?.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var semantics = new TypeDeclaration(codeFile, fullName);
            return new TypeAnalysis(codeFile, scope, syntax, semantics);
        }

        private static TypeAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] EnumStructDeclarationSyntax syntax)
        {
            // Skip any struct that doesn't have a name
            if (!(syntax.Name?.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var semantics = new TypeDeclaration(codeFile, fullName);
            return new TypeAnalysis(codeFile, scope, syntax, semantics);
        }
    }
}