using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumClassDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public EnumKeywordToken EnumKeyword { get; }
        [NotNull] public ClassKeywordToken ClassKeyword { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [CanBeNull] public BaseClassSyntax BaseClass { get; }
        [CanBeNull] public BaseTypesSyntax BaseTypes { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public SyntaxList<InvariantSyntax> Invariants { get; }
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public EnumVariantsSyntax Variants { get; }
        [NotNull] public SyntaxList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public EnumClassDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] EnumKeywordToken enumKeyword,
            [NotNull] ClassKeywordToken classKeyword,
            [NotNull] IIdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [CanBeNull] BaseClassSyntax baseClass,
            [CanBeNull] BaseTypesSyntax baseTypes,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [NotNull] SyntaxList<InvariantSyntax> invariants,
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] EnumVariantsSyntax variants,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [NotNull] ICloseBraceToken closeBrace)
            : base(TextSpan.Covering(enumKeyword.Span, name.Span))
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(enumKeyword), enumKeyword);
            Requires.NotNull(nameof(classKeyword), classKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(invariants), invariants);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(variants), variants);
            Requires.NotNull(nameof(members), members);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Modifiers = modifiers;
            EnumKeyword = enumKeyword;
            ClassKeyword = classKeyword;
            Name = name;
            GenericParameters = genericParameters;
            BaseClass = baseClass;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
            OpenBrace = openBrace;
            Variants = variants;
            CloseBrace = closeBrace;
            Members = members;
        }
    }
}
