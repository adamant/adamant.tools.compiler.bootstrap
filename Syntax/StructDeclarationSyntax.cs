using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class StructDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public IIdentifierOrPrimitiveToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [CanBeNull] public FixedList<ExpressionSyntax> BaseTypes { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<InvariantSyntax> Invariants { get; }
        [NotNull] public FixedList<MemberDeclarationSyntax> Members { get; }

        public StructDeclarationSyntax(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierOrPrimitiveToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [CanBeNull] FixedList<ExpressionSyntax> baseTypes,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<InvariantSyntax> invariants,
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
            Members = members;
        }
    }
}
