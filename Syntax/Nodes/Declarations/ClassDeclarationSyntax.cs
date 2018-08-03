using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ClassDeclarationSyntax : DeclarationSyntax
    {
        public Token AccessModifier { get; }
        public override IdentifierToken Name { get; }

        public ClassDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            AccessModifier = (Token)Children.First();
            Name = Children.OfType<IdentifierToken>().Single();
        }
    }
}
