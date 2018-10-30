using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types
{
    public class EnumStructDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public EnumKeywordToken EnumKeyword { get; }
        [NotNull] public IStructKeywordToken StructKeyword { get; }
        [NotNull] public override IIdentifierToken Name { get; }
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public SyntaxList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public EnumStructDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] EnumKeywordToken enumKeyword,
            [NotNull] IStructKeywordToken structKeyword,
            [NotNull] IIdentifierToken name,
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [NotNull] ICloseBraceToken closeBrace)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(enumKeyword), enumKeyword);
            Requires.NotNull(nameof(structKeyword), structKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(members), members);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Modifiers = modifiers;
            EnumKeyword = enumKeyword;
            StructKeyword = structKeyword;
            Name = name;
            OpenBrace = openBrace;
            CloseBrace = closeBrace;
            Members = members;
        }
    }
}
