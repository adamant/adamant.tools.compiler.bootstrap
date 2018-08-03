using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public abstract class NameSyntax : TypeSyntax
    {
        protected NameSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
