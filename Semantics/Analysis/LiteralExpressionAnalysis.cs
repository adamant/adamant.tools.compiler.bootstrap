using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class LiteralExpressionAnalysis : ExpressionAnalysis
    {
        protected LiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] LiteralExpressionSyntax syntax)
            : base(context, syntax)
        {
        }
    }
}
