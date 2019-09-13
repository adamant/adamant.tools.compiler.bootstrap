using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class ReferenceLifetimeSyntax : TypeSyntax
    {
        public ExpressionSyntax ReferentTypeExpression { get; }
        public SimpleName Lifetime { get; }

        public ReferenceLifetimeSyntax(
            ExpressionSyntax referentTypeExpression,
            TextSpan nameSpan,
            SimpleName lifetime)
            : base(TextSpan.Covering(referentTypeExpression.Span, nameSpan))
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
