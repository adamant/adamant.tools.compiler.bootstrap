using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax Type { get; }
        [NotNull] public LifetimeOperator Operator { get; }
        [NotNull] public ILifetimeNameToken Lifetime { get; }

        public LifetimeTypeSyntax(
            [NotNull] ExpressionSyntax type,
            LifetimeOperator lifetimeOperator,
            [NotNull] ILifetimeNameToken lifetime)
            : base(TextSpan.Covering(type.Span, lifetime.Span))
        {
            Type = type;
            Operator = lifetimeOperator;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{Type}{Operator}{Lifetime}";
        }
    }
}
