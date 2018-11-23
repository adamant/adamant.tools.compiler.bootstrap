using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class StructDeclarationSyntax : TypeDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public IIdentifierOrPrimitiveToken Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [CanBeNull] public FixedList<ExpressionSyntax> BaseTypes { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<ExpressionSyntax> Invariants { get; }
        [NotNull] public FixedList<IMemberDeclarationSyntax> Members { get; }

        public StructDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierOrPrimitiveToken name,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [CanBeNull] FixedList<ExpressionSyntax> baseTypes,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<ExpressionSyntax> invariants,
            [NotNull] FixedList<IMemberDeclarationSyntax> members)
            : base(file, name.Span)
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

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
