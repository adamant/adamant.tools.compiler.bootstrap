using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders
{
    public interface IExpressionAnalysisBuilder
    {
        [ContractAnnotation("expression:null => null; expression:notnull => notnull")]
        ExpressionAnalysis BuildExpression(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [CanBeNull] ExpressionSyntax expression);
    }
}
