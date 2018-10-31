using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Enums;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types
{
    public class EnumStructDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public EnumKeywordToken EnumKeyword { get; }
        [NotNull] public IStructKeywordToken StructKeyword { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public EnumVariantsSyntax Variants { get; }
        [NotNull] public SyntaxList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public EnumStructDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] EnumKeywordToken enumKeyword,
            [NotNull] IStructKeywordToken structKeyword,
            [NotNull] IIdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] EnumVariantsSyntax variants,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [NotNull] ICloseBraceToken closeBrace)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(enumKeyword), enumKeyword);
            Requires.NotNull(nameof(structKeyword), structKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(variants), variants);
            Requires.NotNull(nameof(members), members);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Modifiers = modifiers;
            EnumKeyword = enumKeyword;
            StructKeyword = structKeyword;
            Name = name;
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
            OpenBrace = openBrace;
            Variants = variants;
            CloseBrace = closeBrace;
            Members = members;
        }
    }
}
