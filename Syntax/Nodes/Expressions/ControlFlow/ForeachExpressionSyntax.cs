using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ForeachExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ForeachKeywordToken ForeachKeyword { get; }
        [CanBeNull] public VarKeywordToken VarKeyword { get; }
        [NotNull] public IIdentifierToken Identifier { get; }
        [NotNull] public IInKeywordToken InKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public BlockExpressionSyntax Block { get; }

        public ForeachExpressionSyntax(
            [NotNull] ForeachKeywordToken foreachKeyword,
            [CanBeNull] VarKeywordToken varKeyword,
            [NotNull] IIdentifierToken identifier,
            [NotNull] IInKeywordToken inKeyword,
            [NotNull] ExpressionSyntax expression,
            [NotNull] BlockExpressionSyntax block)
            : base(TextSpan.Covering(foreachKeyword.Span, block.Span))
        {
            Requires.NotNull(nameof(foreachKeyword), foreachKeyword);
            Requires.NotNull(nameof(identifier), identifier);
            Requires.NotNull(nameof(inKeyword), inKeyword);
            Requires.NotNull(nameof(expression), expression);
            Requires.NotNull(nameof(block), block);
            ForeachKeyword = foreachKeyword;
            VarKeyword = varKeyword;
            Identifier = identifier;
            InKeyword = inKeyword;
            Expression = expression;
            Block = block;
        }
    }
}
