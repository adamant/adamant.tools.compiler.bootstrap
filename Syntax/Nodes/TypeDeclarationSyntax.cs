using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class TypeDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<AttributeSyntax> Attributes { get; }
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public TypeKeywordToken TypeKeyword { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [CanBeNull] public BaseTypesSyntax BaseTypes { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public SyntaxList<InvariantSyntax> Invariants { get; }
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public SyntaxList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public TypeDeclarationSyntax(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] TypeKeywordToken typeKeyword,
            [NotNull] IIdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [CanBeNull] BaseTypesSyntax baseTypes,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [NotNull] SyntaxList<InvariantSyntax> invariants,
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [NotNull] ICloseBraceToken closeBrace)
            : base(TextSpan.Covering(typeKeyword.Span, name.Span))
        {
            Requires.NotNull(nameof(attributes), attributes);
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(typeKeyword), typeKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(invariants), invariants);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(members), members);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Attributes = attributes;
            Modifiers = modifiers;
            TypeKeyword = typeKeyword;
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