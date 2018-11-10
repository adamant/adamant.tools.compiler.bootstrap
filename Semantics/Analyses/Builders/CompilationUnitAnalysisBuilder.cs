using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
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
            {
                var scope = new CompilationUnitScope(compilationUnit);
                var context = new AnalysisContext(compilationUnit.CodeFile, scope);
                var fileNamespace = declarationBuilder.BuildFileNamespace(context, compilationUnit.FileNamespace);
                yield return new CompilationUnitAnalysis(scope, compilationUnit, fileNamespace);
            }
        }
    }
}
