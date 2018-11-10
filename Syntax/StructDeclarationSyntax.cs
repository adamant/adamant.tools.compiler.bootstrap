using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class StructDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<AttributeSyntax> Attributes { get; }
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IStructKeywordToken StructKeyword { get; }
        [NotNull] public IIdentifierOrPrimitiveTokenPlace Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [CanBeNull] public BaseTypesSyntax BaseTypes { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public SyntaxList<InvariantSyntax> Invariants { get; }
        [NotNull] public IOpenBraceTokenPlace OpenBrace { get; }
        [NotNull] public FixedList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public ICloseBraceTokenPlace CloseBrace { get; }

        public StructDeclarationSyntax(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IStructKeywordToken structKeyword,
            [NotNull] IIdentifierOrPrimitiveTokenPlace name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [CanBeNull] BaseTypesSyntax baseTypes,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [NotNull] SyntaxList<InvariantSyntax> invariants,
            [NotNull] IOpenBraceTokenPlace openBrace,
            [NotNull] FixedList<MemberDeclarationSyntax> members,
            [NotNull] ICloseBraceTokenPlace closeBrace)
            : base(TextSpan.Covering(structKeyword.Span, name.Span))
        {
            Requires.NotNull(nameof(attributes), attributes);
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(structKeyword), structKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(invariants), invariants);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(members), members);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Attributes = attributes;
            Modifiers = modifiers;
            StructKeyword = structKeyword;
            Name = name;
            GenericParameters = genericParameters;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
            OpenBrace = openBrace;
            Members = members;
            CloseBrace = closeBrace;
        }
    }
}
