using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : DeclarationSyntax
    {
        protected FunctionDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
