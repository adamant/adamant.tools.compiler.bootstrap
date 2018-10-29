using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class UnsafeExpression : ExpressionSyntax
    {
        [NotNull] public UnsafeKeywordToken UnsafeKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public UnsafeExpression(
            [NotNull] UnsafeKeywordToken unsafeKeyword,
            [NotNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(unsafeKeyword.Span, expression.Span))
        {
            Requires.NotNull(nameof(unsafeKeyword), unsafeKeyword);
            Requires.NotNull(nameof(expression), expression);
            UnsafeKeyword = unsafeKeyword;
            Expression = expression;
        }
    }
}
