using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ForeachExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IForeachKeywordToken ForeachKeyword { get; }
        [CanBeNull] public IVarKeywordToken VarKeyword { get; }
        [NotNull] public IIdentifierTokenPlace Identifier { get; }
        [CanBeNull] public IColonToken Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public IInKeywordTokenPlace InKeyword { get; }
        [NotNull] public ExpressionSyntax InExpression { get; }
        [NotNull] public BlockSyntax Block { get; }

        public ForeachExpressionSyntax(
            [NotNull] IForeachKeywordToken foreachKeyword,
            [CanBeNull] IVarKeywordToken varKeyword,
            [NotNull] IIdentifierTokenPlace identifier,
            [CanBeNull] IColonToken colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [NotNull] IInKeywordTokenPlace inKeyword,
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
            Colon = colon;
            TypeExpression = typeExpression;
        }
    }
}
