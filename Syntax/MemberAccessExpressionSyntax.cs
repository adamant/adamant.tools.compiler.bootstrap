using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public IOperatorToken AccessOperator { get; }
        [NotNull] public IMemberNameTokenPlace Member { get; }

        public MemberAccessExpressionSyntax(
            [NotNull] ExpressionSyntax expression,
            [NotNull] IOperatorToken accessOperator,
            [NotNull] IMemberNameTokenPlace member)
            : base(TextSpan.Covering(expression.Span, member.Span))
        {
            Expression = expression;
            AccessOperator = accessOperator;
            Member = member;
        }

        public override string ToString()
        {
            return $"{Expression}.{Member}";
        }
    }
}
