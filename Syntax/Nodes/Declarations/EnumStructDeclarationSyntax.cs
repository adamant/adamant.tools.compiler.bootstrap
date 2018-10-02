using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class EnumStructDeclarationSyntax : DeclarationSyntax
    {
        public Token AccessModifier { get; }
        public override IdentifierToken Name { get; }
        public IReadOnlyList<MemberDeclarationSyntax> Members { get; }

        public EnumStructDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            AccessModifier = (Token)Children.First();
            Name = Children.OfType<IdentifierToken>().Single();
            Members = Children.OfType<MemberDeclarationSyntax>().ToList().AsReadOnly();
        }
    }
}
