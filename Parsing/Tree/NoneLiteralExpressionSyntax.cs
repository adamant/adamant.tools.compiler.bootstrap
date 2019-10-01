using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NoneLiteralExpressionSyntax : LiteralExpressionSyntax, INoneLiteralExpressionSyntax
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
