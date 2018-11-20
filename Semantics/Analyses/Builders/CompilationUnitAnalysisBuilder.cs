using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders
{
    public class CompilationUnitAnalysisBuilder
    {
        [NotNull] private readonly DeclarationAnalysisBuilder declarationBuilder;

        public CompilationUnitAnalysisBuilder(
            [NotNull] DeclarationAnalysisBuilder declarationBuilder)
        {
            this.declarationBuilder = declarationBuilder;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CompilationUnitAnalysis> BuildPackage([NotNull] PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
                yield return BuildCompilationUnit(compilationUnit);
        }

        [NotNull]
        private CompilationUnitAnalysis BuildCompilationUnit([NotNull] CompilationUnitSyntax compilationUnit)
        {
            var scope = new CompilationUnitScope(compilationUnit);
            var context = new AnalysisContext(compilationUnit.CodeFile, scope);
            var name = compilationUnit.ImplicitNamespaceName;
            var declarations = new List<DeclarationAnalysis>();
            foreach (var declaration in compilationUnit.Declarations)
            {
                var analysis = declarationBuilder.BuildDeclaration(context, name, declaration);
                if (analysis != null)
                    declarations.Add(analysis);
            }
            return new CompilationUnitAnalysis(scope, compilationUnit, declarations.ToFixedList());
        }
    }
}
