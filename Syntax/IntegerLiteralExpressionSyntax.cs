using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class IntegerLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public IIntegerLiteralToken Literal { get; }

        public IntegerLiteralExpressionSyntax([NotNull] IIntegerLiteralToken literal)
            : base(literal.Span)
        {
            Requires.NotNull(nameof(literal), literal);
            Literal = literal;
        }
    }
}
