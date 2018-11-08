using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class LoopExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public LoopKeywordToken LoopKeyword { get; }
        [NotNull] public BlockSyntax Block { get; }

        public LoopExpressionSyntax(
            [NotNull] LoopKeywordToken loopKeyword,
            [NotNull] BlockSyntax block)
            : base(TextSpan.Covering(loopKeyword.Span, block.Span))
        {
            Requires.NotNull(nameof(loopKeyword), loopKeyword);
            Requires.NotNull(nameof(block), block);
            LoopKeyword = loopKeyword;
            Block = block;
        }
    }
}
