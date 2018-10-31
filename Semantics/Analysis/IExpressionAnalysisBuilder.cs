using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public interface IExpressionAnalysisBuilder
    {
        [NotNull]
        ExpressionAnalysis BuildExpression(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] ExpressionSyntax expression);
    }
}
