using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public ILifetimeOperatorToken Operator { get; }
        [NotNull] public ILifetimeNameTokenPlace Lifetime { get; }

        public LifetimeTypeSyntax(
            [NotNull] ExpressionSyntax typeExpression,
            [NotNull] ILifetimeOperatorToken @operator,
            [NotNull] ILifetimeNameTokenPlace lifetime)
            : base(TextSpan.Covering(typeExpression.Span, lifetime.Span))
        {
            Requires.NotNull(nameof(typeExpression), typeExpression);
            Requires.NotNull(nameof(@operator), @operator);
            Requires.NotNull(nameof(lifetime), lifetime);
            TypeExpression = typeExpression;
            Operator = @operator;
            Lifetime = lifetime;
        }
    }
}
