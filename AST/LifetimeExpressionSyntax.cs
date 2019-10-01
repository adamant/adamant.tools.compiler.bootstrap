using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeExpressionSyntax : ExpressionSyntax, ILifetimeExpressionSyntax
    {
        public SimpleName Lifetime { get; }

        public LifetimeExpressionSyntax(TextSpan span, SimpleName lifetime)
            : base(span)
        {
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"${Lifetime}";
        }
    }
}
