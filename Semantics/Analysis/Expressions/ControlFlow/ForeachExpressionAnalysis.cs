using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow
{
    public class ForeachExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new ForeachExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis InExpression { get; }
        [NotNull] public BlockAnalysis Block { get; }

        public ForeachExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ForeachExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis inExpression,
            [NotNull] BlockAnalysis block)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(inExpression), inExpression);
            Requires.NotNull(nameof(block), block);
            Syntax = syntax;
            Block = block;
            InExpression = inExpression;
        }
    }
}
