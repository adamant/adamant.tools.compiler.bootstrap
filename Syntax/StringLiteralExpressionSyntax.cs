using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public IStringLiteralToken Literal { get; }
        [NotNull] public string Value => Literal.Value;

        public StringLiteralExpressionSyntax([NotNull] IStringLiteralToken literal)
            : base(literal.Span)
        {
            Requires.NotNull(nameof(literal), literal);
            Literal = literal;
        }
    }
}
