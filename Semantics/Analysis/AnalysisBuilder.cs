using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class AnalysisBuilder
    {
        [NotNull] private readonly CompilationUnitAnalysisBuilder compilationUnitAnalysisBuilder;

        public AnalysisBuilder([NotNull] NameBuilder nameBuilder)
        {
            StatementAnalysisBuilder statementBuilder = null;
            var expressionBuilder = new ExpressionAnalysisBuilder(() => statementBuilder, nameBuilder);
            statementBuilder = new StatementAnalysisBuilder(expressionBuilder);
            var declarationBuilder = new DeclarationAnalysisBuilder(nameBuilder, expressionBuilder, statementBuilder);
            compilationUnitAnalysisBuilder = new CompilationUnitAnalysisBuilder(nameBuilder, declarationBuilder);
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CompilationUnitAnalysis> Build([NotNull] PackageSyntax packageSyntax)
        {
            return compilationUnitAnalysisBuilder.Build(packageSyntax);
        }
    }
}
