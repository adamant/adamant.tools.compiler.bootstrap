using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LiteralExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ILiteralToken Literal { get; }

        public LiteralExpressionSyntax([NotNull] ILiteralToken literal)
            : base(literal.Span)
        {
            Literal = literal;
        }

        [NotNull]
        public override string ToString()
        {
            return Literal.ToString().NotNull();
        }
    }
}
