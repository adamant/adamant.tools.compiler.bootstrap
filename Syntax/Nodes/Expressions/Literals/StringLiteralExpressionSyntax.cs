using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public StringLiteralToken StringLiteral { get; }
        [NotNull] public string Value => StringLiteral.Value;

        public StringLiteralExpressionSyntax([NotNull] StringLiteralToken stringLiteral)
            : base(stringLiteral.Span)
        {
            Requires.NotNull(nameof(stringLiteral), stringLiteral);
            StringLiteral = stringLiteral;
        }
    }
}
