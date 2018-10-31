using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Literals;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class BooleanLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public BooleanLiteralToken Literal { get; }
        [NotNull] public bool Value => Literal.Value;

        public BooleanLiteralExpressionSyntax([NotNull] BooleanLiteralToken literal)
            : base(literal.Span)
        {
            Requires.NotNull(nameof(literal), literal);
            Literal = literal;
        }
    }
}
