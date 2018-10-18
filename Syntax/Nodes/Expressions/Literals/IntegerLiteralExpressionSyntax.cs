using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class IntegerLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public IntegerLiteralToken IntegerLiteral { get; }

        public IntegerLiteralExpressionSyntax([NotNull] IntegerLiteralToken integerLiteral)
            : base(integerLiteral.Span)
        {
            Requires.NotNull(nameof(integerLiteral), integerLiteral);
            IntegerLiteral = integerLiteral;
        }
    }
}
