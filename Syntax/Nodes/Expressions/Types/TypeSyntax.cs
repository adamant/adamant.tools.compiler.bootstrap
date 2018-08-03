using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public abstract class TypeSyntax : ExpressionSyntax
    {
        protected TypeSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
