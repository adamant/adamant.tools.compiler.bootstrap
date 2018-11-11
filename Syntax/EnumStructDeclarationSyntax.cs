using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumStructDeclarationSyntax : TypeDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [CanBeNull] public FixedList<ExpressionSyntax> BaseTypes { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<InvariantSyntax> Invariants { get; }
        [NotNull] public EnumVariantsSyntax Variants { get; }
        [NotNull] public FixedList<MemberDeclarationSyntax> Members { get; }

        public EnumStructDeclarationSyntax(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierToken name,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [CanBeNull] FixedList<ExpressionSyntax> baseTypes,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<InvariantSyntax> invariants,
            [NotNull] EnumVariantsSyntax variants,
            [NotNull] FixedList<MemberDeclarationSyntax> members)
            : base(name.Span)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            Name = name;
            GenericParameters = genericParameters;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
            Variants = variants;
            Members = members;
        }
    }
}
