using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Fakes
{
    public class FakeExpressionAnalysisBuilder : IExpressionAnalysisBuilder
    {
        [NotNull]
        public ExpressionAnalysis BuildExpression(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] ExpressionSyntax expression)
        {
            return FakeAnalysis.Expression();
        }
    }
}
