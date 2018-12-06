using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        public ExpressionSyntax ReferentTypeExpression { get; }
        public LifetimeOperator Operator { get; }
        public ILifetimeNameToken Lifetime { get; }

        public LifetimeTypeSyntax(
            ExpressionSyntax referentTypeExpression,
            LifetimeOperator lifetimeOperator,
            ILifetimeNameToken lifetime)
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
