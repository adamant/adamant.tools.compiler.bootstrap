using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NoneLiteralExpressionSyntax : LiteralExpressionSyntax, INoneLiteralExpressionSyntax
    {
        public NoneLiteralExpressionSyntax(TextSpan span)
            : base(span, ExpressionSemantics.Copy)
        {
        }

        public override string ToString()
        {
            return "none";
        }
    }
}
