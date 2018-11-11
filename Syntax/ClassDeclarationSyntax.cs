using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ClassDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [CanBeNull] public ExpressionSyntax BaseClass { get; }
        [CanBeNull] public FixedList<ExpressionSyntax> BaseTypes { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<InvariantSyntax> Invariants { get; }
        [NotNull] public FixedList<MemberDeclarationSyntax> Members { get; }

        public ClassDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [CanBeNull] ExpressionSyntax baseClass,
            [CanBeNull] FixedList<ExpressionSyntax> baseTypes,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<InvariantSyntax> invariants,
            [NotNull] FixedList<MemberDeclarationSyntax> members)
            : base(name.Span)
        {
            Modifiers = modifiers;
            Name = name;
            GenericParameters = genericParameters;
            BaseClass = baseClass;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
            Members = members;
        }
    }
}
