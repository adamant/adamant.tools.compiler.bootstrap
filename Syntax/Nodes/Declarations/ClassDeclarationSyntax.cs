using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ClassDeclarationSyntax : MemberDeclarationSyntax
    {
        public SimpleToken AccessModifier { get; }
        public SimpleToken ClassKeyword { get; }
        public override IdentifierToken Name { get; }
        public SimpleToken OpenBrace { get; }
        public SyntaxList<MemberDeclarationSyntax> Members { get; }
        public SimpleToken CloseBrace { get; }

        public ClassDeclarationSyntax(
            SimpleToken accessModifier,
            SimpleToken classKeyword,
            IdentifierToken name,
            SimpleToken openBrace,
            SyntaxList<MemberDeclarationSyntax> members,
            SimpleToken closeBrace)
        {
            AccessModifier = accessModifier;
            ClassKeyword = classKeyword;
            Name = name;
            OpenBrace = openBrace;
            Members = members;
            CloseBrace = closeBrace;
        }
    }
}
