using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public OperatorToken AccessOperator { get; }
        [NotNull] public IMemberNameToken Member { get; }

        public MemberAccessExpressionSyntax(
            [NotNull] ExpressionSyntax expression,
            [NotNull] OperatorToken accessOperator,
            [NotNull] IMemberNameToken member)
            : base(TextSpan.Covering(expression.Span, member.Span))
        {
            Requires.NotNull(nameof(expression), expression);
            Requires.NotNull(nameof(accessOperator), accessOperator);
            Requires.NotNull(nameof(member), member);
            Expression = expression;
            AccessOperator = accessOperator;
            Member = member;
        }
    }
}
