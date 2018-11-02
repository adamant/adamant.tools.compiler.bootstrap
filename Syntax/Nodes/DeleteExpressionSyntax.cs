using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class DeleteExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public DeleteKeywordToken DeleteKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public DeleteExpressionSyntax(
            [NotNull] DeleteKeywordToken deleteKeyword,
            [NotNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(deleteKeyword.Span, expression.Span))
        {
            Requires.NotNull(nameof(deleteKeyword), deleteKeyword);
            Requires.NotNull(nameof(expression), expression);
            DeleteKeyword = deleteKeyword;
            Expression = expression;
        }
    }
}
