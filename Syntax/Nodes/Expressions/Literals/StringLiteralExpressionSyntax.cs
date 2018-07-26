using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class StringLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        public StringLiteralExpressionSyntax(Token token)
            : base(token.Yield())
        {
        }
    }
}
