using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        public StringLiteralToken StringLiteral { get; }

        public StringLiteralExpressionSyntax(StringLiteralToken stringLiteral)
        {
            StringLiteral = stringLiteral;
        }
    }
}
