using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class WhileExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new WhileExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Condition { get; }
        [NotNull] public BlockAnalysis Block { get; }

        public WhileExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] WhileExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis condition,
            [NotNull] BlockAnalysis block)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(condition), condition);
            Requires.NotNull(nameof(block), block);
            Syntax = syntax;
            Condition = condition;
            Block = block;
        }
    }
}
