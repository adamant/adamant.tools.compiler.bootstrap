using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class NamespaceSyntax : DeclarationSyntax
    {
        public NamespaceSyntax(IEnumerable<DeclarationSyntax> children)
            : base(children)
        {
        }
    }
}
