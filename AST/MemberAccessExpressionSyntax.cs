using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        public AccessOperator AccessOperator { get; }
        [NotNull] public IMemberNameToken Member { get; }

        public MemberAccessExpressionSyntax(
            [NotNull] ExpressionSyntax expression,
            AccessOperator accessOperator,
            [NotNull] IMemberNameToken member)
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
