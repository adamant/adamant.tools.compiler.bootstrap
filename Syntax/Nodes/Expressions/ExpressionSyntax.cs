using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public abstract class ExpressionSyntax : SyntaxBranchNode
    {
        protected ExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }

        protected ExpressionSyntax(params SyntaxNode[] children)
            : base(children)
        {
        }
    }
}
