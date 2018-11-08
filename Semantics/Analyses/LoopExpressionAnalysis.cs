using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class LoopExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new LoopExpressionSyntax Syntax { get; }
        [NotNull] public BlockAnalysis Block { get; }

        public LoopExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] LoopExpressionSyntax syntax,
            [NotNull] BlockAnalysis block)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(block), block);
            Syntax = syntax;
            Block = block;
        }
    }
}
