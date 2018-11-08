using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class IntegerLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public IntegerLiteralToken Literal { get; }

        public IntegerLiteralExpressionSyntax([NotNull] IntegerLiteralToken literal)
            : base(literal.Span)
        {
            Requires.NotNull(nameof(literal), literal);
            Literal = literal;
        }
    }
}