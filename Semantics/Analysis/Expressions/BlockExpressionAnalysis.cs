using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class BlockExpressionAnalysis : ExpressionAnalysis
    {
        public BlockExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BlockExpressionSyntax syntax)
            : base(context, syntax)
        {
        }
    }
}
