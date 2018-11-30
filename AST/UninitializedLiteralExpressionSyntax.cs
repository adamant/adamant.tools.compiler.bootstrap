using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UninitializedLiteralExpressionSyntax : LiteralExpressionSyntax
    {
        public UninitializedLiteralExpressionSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "uninitialized";
        }
    }
}
