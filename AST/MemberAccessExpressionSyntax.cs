using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        /// <summary>
        /// This expression is null for implicit member access i.e. self and enums
        /// </summary>
        [CanBeNull] public ExpressionSyntax Expression { get; }
        public AccessOperator AccessOperator { get; }
        [NotNull] public IMemberNameToken Member { get; }

        public MemberAccessExpressionSyntax(
            TextSpan span,
            [CanBeNull] ExpressionSyntax expression,
            AccessOperator accessOperator,
            [NotNull] IMemberNameToken member)
            : base(span)
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
