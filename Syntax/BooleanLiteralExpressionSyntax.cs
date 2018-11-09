using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BooleanLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public IBooleanLiteralToken Literal { get; }
        [NotNull] public bool Value => Literal.Value;

        public BooleanLiteralExpressionSyntax([NotNull] IBooleanLiteralToken literal)
            : base(literal.Span)
        {
            Requires.NotNull(nameof(literal), literal);
            Literal = literal;
        }
    }
}
