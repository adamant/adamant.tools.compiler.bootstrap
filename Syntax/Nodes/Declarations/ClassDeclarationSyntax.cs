using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ClassDeclarationSyntax : DeclarationSyntax
    {
        public Token AccessModifier => (Token)Children.First();
        public override IdentifierToken Name => Children.OfType<IdentifierToken>().Single();

        public ClassDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
