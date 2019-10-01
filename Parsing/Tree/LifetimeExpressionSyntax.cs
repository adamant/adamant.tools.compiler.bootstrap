using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class LifetimeExpressionSyntax : ExpressionSyntax, ILifetimeExpressionSyntax
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
