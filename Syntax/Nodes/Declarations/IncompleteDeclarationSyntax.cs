using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class IncompleteDeclarationSyntax : DeclarationSyntax
    {
        public override IdentifierToken Name => Children.OfType<IdentifierToken>().First();

        public IncompleteDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
