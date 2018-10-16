using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public StringLiteralToken StringLiteral { get; }

        public StringLiteralExpressionSyntax([NotNull] StringLiteralToken stringLiteral)
        {
            Requires.NotNull(nameof(stringLiteral), stringLiteral);
            StringLiteral = stringLiteral;
        }
    }
}
