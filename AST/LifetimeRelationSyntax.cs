using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeRelationSyntax : TypeSyntax
    {
        public ExpressionSyntax ReferentTypeExpression { get; }
        public LifetimeRelationOperator Operator { get; }
        public ILifetimeNameToken Lifetime { get; }

        public LifetimeRelationSyntax(
            ExpressionSyntax referentTypeExpression,
            LifetimeRelationOperator lifetimeOperator,
            ILifetimeNameToken lifetime)
            : base(TextSpan.Covering(referentTypeExpression.Span, lifetime.Span))
        {
            ReferentTypeExpression = referentTypeExpression;
            Operator = lifetimeOperator;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{ReferentTypeExpression}${Operator.ToSymbolString()}{Lifetime}";
        }
    }
}
