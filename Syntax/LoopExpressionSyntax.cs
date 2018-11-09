using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class LoopExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ILoopKeywordToken LoopKeyword { get; }
        [NotNull] public BlockSyntax Block { get; }

        public LoopExpressionSyntax(
            [NotNull] ILoopKeywordToken loopKeyword,
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
