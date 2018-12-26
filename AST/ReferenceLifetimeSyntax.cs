using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ReferenceLifetimeSyntax : TypeSyntax
    {
        public ExpressionSyntax ReferentTypeExpression { get; }
        public ILifetimeNameToken Lifetime { get; }

        public ReferenceLifetimeSyntax(
            ExpressionSyntax referentTypeExpression,
            ILifetimeNameToken lifetime)
            : base(TextSpan.Covering(referentTypeExpression.Span, lifetime.Span))
        {
            ReferentTypeExpression = referentTypeExpression;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{ReferentTypeExpression}${Lifetime}";
        }
    }
}
