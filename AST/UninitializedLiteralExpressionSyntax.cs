using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [VisitorNotSupported("Only implemented in parser")]
    public sealed class UninitializedLiteralExpressionSyntax : LiteralExpressionSyntax
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
