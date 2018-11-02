using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public StringLiteralToken Literal { get; }
        [NotNull] public string Value => Literal.Value;

        public StringLiteralExpressionSyntax([NotNull] StringLiteralToken literal)
            : base(literal.Span)
        {
            Requires.NotNull(nameof(literal), literal);
            Literal = literal;
        }
    }
}
