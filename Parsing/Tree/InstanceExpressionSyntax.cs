using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InstanceExpressionSyntax : ExpressionSyntax, IInstanceExpressionSyntax
    {
        protected InstanceExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
