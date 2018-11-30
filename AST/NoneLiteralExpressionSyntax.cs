using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NoneLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        public NoneLiteralExpressionSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "none";
        }
    }
}
