using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public LifetimeOperator Operator { get; }
        [NotNull] public ILifetimeNameToken Lifetime { get; }

        public LifetimeTypeSyntax(
            [NotNull] ExpressionSyntax typeExpression,
            LifetimeOperator lifetimeOperator,
            [NotNull] ILifetimeNameToken lifetime)
            : base(TextSpan.Covering(typeExpression.Span, lifetime.Span))
        {
            TypeExpression = typeExpression;
            Operator = lifetimeOperator;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{TypeExpression}{Operator}{Lifetime}";
        }
    }
}
