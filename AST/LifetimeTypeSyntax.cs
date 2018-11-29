using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax ReferentTypeExpression { get; }
        [NotNull] public LifetimeOperator Operator { get; }
        [NotNull] public ILifetimeNameToken Lifetime { get; }

        public LifetimeTypeSyntax(
            [NotNull] ExpressionSyntax referentTypeExpression,
            LifetimeOperator lifetimeOperator,
            [NotNull] ILifetimeNameToken lifetime)
            : base(TextSpan.Covering(referentTypeExpression.Span, lifetime.Span))
        {
            ReferentTypeExpression = referentTypeExpression;
            Operator = lifetimeOperator;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{ReferentTypeExpression}{Operator}{Lifetime}";
        }
    }
}
