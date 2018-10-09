using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ClassDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull]
        public AccessModifierSyntax AccessModifier { get; }

        [CanBeNull]
        public ClassKeywordToken ClassKeyword { get; }

        [CanBeNull]
        public override IdentifierToken Name { get; }

        [CanBeNull]
        public OpenBraceToken OpenBrace { get; }

        [NotNull]
        public SyntaxList<MemberDeclarationSyntax> Members { get; }

        [CanBeNull]
        public CloseBraceToken CloseBrace { get; }

        public ClassDeclarationSyntax(
            [NotNull] AccessModifierSyntax accessModifier,
            [CanBeNull] ClassKeywordToken classKeyword,
            [CanBeNull] IdentifierToken name,
            [CanBeNull] OpenBraceToken openBrace,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [CanBeNull] CloseBraceToken closeBrace)
        {
            Requires.NotNull(nameof(accessModifier), accessModifier);
            Requires.NotNull(nameof(members), members);
            AccessModifier = accessModifier;
            ClassKeyword = classKeyword;
            Name = name;
            OpenBrace = openBrace;
            Members = members;
            CloseBrace = closeBrace;
        }
    }
}
