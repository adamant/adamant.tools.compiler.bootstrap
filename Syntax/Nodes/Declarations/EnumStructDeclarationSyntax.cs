using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class EnumStructDeclarationSyntax : DeclarationSyntax
    {
        public SimpleToken AccessModifier { get; }
        public SimpleToken EnumKeyword { get; }
        public SimpleToken StructKeyword { get; }
        public override IdentifierToken Name { get; }
        public SimpleToken OpenBrace { get; }
        public SyntaxList<MemberDeclarationSyntax> Members { get; }
        public SimpleToken CloseBrace { get; }

        public EnumStructDeclarationSyntax(
            SimpleToken accessModifier,
            SimpleToken enumKeyword,
            SimpleToken structKeyword,
            IdentifierToken name,
            SimpleToken openBrace,
            SyntaxList<MemberDeclarationSyntax> members,
            SimpleToken closeBrace)
        {
            AccessModifier = accessModifier;
            EnumKeyword = enumKeyword;
            StructKeyword = structKeyword;
            Name = name;
            OpenBrace = openBrace;
            CloseBrace = closeBrace;
            Members = members;
        }
    }
}
