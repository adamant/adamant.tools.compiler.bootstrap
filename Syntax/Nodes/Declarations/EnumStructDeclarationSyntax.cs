using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class EnumStructDeclarationSyntax : DeclarationSyntax
    {
        [NotNull]
        public AccessModifierSyntax AccessModifier { get; }

        [CanBeNull]
        public EnumKeywordToken EnumKeyword { get; }

        [CanBeNull]
        public StructKeywordToken StructKeyword { get; }

        [CanBeNull]
        public override IdentifierToken Name { get; }

        [CanBeNull]
        public OpenBraceToken OpenBrace { get; }

        [NotNull]
        public SyntaxList<MemberDeclarationSyntax> Members { get; }

        [CanBeNull]
        public CloseBraceToken CloseBrace { get; }

        public EnumStructDeclarationSyntax(
            [NotNull] AccessModifierSyntax accessModifier,
            [CanBeNull] EnumKeywordToken enumKeyword,
            [CanBeNull] StructKeywordToken structKeyword,
            [CanBeNull] IdentifierToken name,
            [CanBeNull] OpenBraceToken openBrace,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [CanBeNull] CloseBraceToken closeBrace)
        {
            Requires.NotNull(nameof(accessModifier), accessModifier);
            Requires.NotNull(nameof(members), members);
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
