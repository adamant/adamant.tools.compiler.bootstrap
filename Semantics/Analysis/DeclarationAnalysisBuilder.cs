using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class DeclarationAnalysisBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;

        public DeclarationAnalysisBuilder([NotNull] NameBuilder nameBuilder)
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

        private static FunctionDeclarationAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] FunctionDeclarationSyntax syntax)
        {
            // Skip any function that doesn't have a name
            if (!(syntax.Name?.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var parameters = syntax.Parameters.Nodes().Select(Build);
            var semantics = new FunctionDeclaration(codeFile, fullName, parameters);
            return new FunctionDeclarationAnalysis(codeFile, scope, syntax, semantics);
        }

        [NotNull]
        private static Parameter Build([NotNull] ParameterSyntax parameter)
        {
            return new Parameter(parameter.VarKeyword != null, parameter.Name?.Value);
        }

        private static TypeDeclarationAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] ClassDeclarationSyntax syntax)
        {
            // Skip any class that doesn't have a name
            if (!(syntax.Name?.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var semantics = new TypeDeclaration(codeFile, fullName);
            return new TypeDeclarationAnalysis(codeFile, scope, syntax, semantics);
        }

        private static TypeDeclarationAnalysis PrepareForAnalysis(
            [NotNull] CodeFile codeFile,
            [NotNull] LexicalScope scope,
            [NotNull] Name @namespace,
            [NotNull] EnumStructDeclarationSyntax syntax)
        {
            // Skip any struct that doesn't have a name
            if (!(syntax.Name?.Value is string name)) return null;

            var fullName = @namespace.Qualify(name);
            var semantics = new TypeDeclaration(codeFile, fullName);
            return new TypeDeclarationAnalysis(codeFile, scope, syntax, semantics);
        }
    }
}
