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
        [NotNull] public ExpressionSyntax InExpression { get; }
        [NotNull] public BlockSyntax Block { get; }

        public ForeachExpressionSyntax(
            [NotNull] ForeachKeywordToken foreachKeyword,
            [CanBeNull] VarKeywordToken varKeyword,
            [NotNull] IIdentifierToken identifier,
            [NotNull] IInKeywordToken inKeyword,
            [NotNull] ExpressionSyntax inExpression,
            [NotNull] BlockSyntax block)
            : base(TextSpan.Covering(foreachKeyword.Span, block.Span))
        {
            Requires.NotNull(nameof(foreachKeyword), foreachKeyword);
            Requires.NotNull(nameof(identifier), identifier);
            Requires.NotNull(nameof(inKeyword), inKeyword);
            Requires.NotNull(nameof(inExpression), inExpression);
            Requires.NotNull(nameof(block), block);
            ForeachKeyword = foreachKeyword;
            VarKeyword = varKeyword;
            Identifier = identifier;
            InKeyword = inKeyword;
            InExpression = inExpression;
            Block = block;
        }
    }
}
