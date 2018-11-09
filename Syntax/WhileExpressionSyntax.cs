using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class WhileExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IWhileKeywordToken WhileKeyword { get; }
        [NotNull] public ExpressionSyntax Condition { get; }
        [NotNull] public BlockSyntax Block { get; }

        public WhileExpressionSyntax(
            [NotNull] IWhileKeywordToken whileKeyword,
            [NotNull] ExpressionSyntax condition,
            [NotNull] BlockSyntax block)
            : base(TextSpan.Covering(whileKeyword.Span, block.Span))
        {
            Requires.NotNull(nameof(whileKeyword), whileKeyword);
            Requires.NotNull(nameof(condition), condition);
            Requires.NotNull(nameof(block), block);
            WhileKeyword = whileKeyword;
            Condition = condition;
            Block = block;
        }
    }
}
