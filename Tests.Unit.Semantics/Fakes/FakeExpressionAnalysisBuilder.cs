using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Fakes
{
    public class FakeExpressionAnalysisBuilder : IExpressionAnalysisBuilder
    {
        [NotNull]
        public ExpressionAnalysis BuildExpression(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] ExpressionSyntax expression)
        {
            return FakeAnalysis.Expression();
        }
    }
}
