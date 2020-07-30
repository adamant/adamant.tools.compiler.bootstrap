using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class LiteralExpressionSyntax : ExpressionSyntax, ILiteralExpressionSyntax
    {
        protected LiteralExpressionSyntax(TextSpan span, ExpressionSemantics? semantics = null)
            : base(span, semantics)
        {
        }
    }
}
