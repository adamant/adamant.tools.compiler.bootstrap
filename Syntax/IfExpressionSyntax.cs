using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IIfKeywordToken IfKeyword { get; }
        [NotNull] public ExpressionSyntax Condition { get; }
        [NotNull] public ExpressionBlockSyntax ThenBlock { get; }
        [CanBeNull] public ElseClauseSyntax ElseClause { get; }

        public IfExpressionSyntax(
            [NotNull] IIfKeywordToken ifKeyword,
            [NotNull] ExpressionSyntax condition,
            [NotNull] ExpressionBlockSyntax thenBlock,
            [CanBeNull] ElseClauseSyntax elseClause)
            : base(TextSpan.Covering(ifKeyword.Span, thenBlock.Span, elseClause?.Span))
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }

        public override string ToString()
        {
            if (ElseClause != null)
                return $"if {Condition} {ThenBlock} {ElseClause}";
            return $"if {Condition} {ThenBlock}";
        }
    }
}
