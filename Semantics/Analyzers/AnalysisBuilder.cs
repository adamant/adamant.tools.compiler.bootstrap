using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Builders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class AnalysisBuilder
    {
        [NotNull] private readonly CompilationUnitAnalysisBuilder compilationUnitAnalysisBuilder;

        public AnalysisBuilder([NotNull] NameBuilder nameBuilder)
        {
            StatementAnalysisBuilder statementBuilder = null;
            var expressionBuilder = new ExpressionAnalysisBuilder(() => statementBuilder);
            statementBuilder = new StatementAnalysisBuilder(expressionBuilder);
            var declarationBuilder = new DeclarationAnalysisBuilder(expressionBuilder, statementBuilder, nameBuilder);
            compilationUnitAnalysisBuilder = new CompilationUnitAnalysisBuilder(declarationBuilder);
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CompilationUnitAnalysis> Build([NotNull] PackageSyntax packageSyntax)
        {
            return compilationUnitAnalysisBuilder.BuildPackage(packageSyntax);
        }
    }
}
