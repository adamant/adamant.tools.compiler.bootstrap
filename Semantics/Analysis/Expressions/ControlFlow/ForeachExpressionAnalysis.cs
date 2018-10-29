using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow
{
    public class ForeachExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new ForeachExpressionSyntax Syntax { get; }

        public ForeachExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ForeachExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
