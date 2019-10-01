using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class LiteralExpressionSyntax : ExpressionSyntax
    {
        protected LiteralExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
