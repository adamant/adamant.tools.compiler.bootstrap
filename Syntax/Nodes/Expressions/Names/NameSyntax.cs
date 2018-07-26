using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names
{
    public abstract class NameSyntax : ExpressionSyntax
    {
        protected NameSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
