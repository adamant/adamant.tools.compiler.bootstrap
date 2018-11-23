using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class EnumClassDeclarationSyntax : TypeDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [CanBeNull] public ExpressionSyntax BaseClass { get; }
        [CanBeNull] public FixedList<ExpressionSyntax> BaseTypes { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<ExpressionSyntax> Invariants { get; }
        [NotNull] public FixedList<EnumVariantSyntax> Variants { get; }

        public EnumClassDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [CanBeNull] ExpressionSyntax baseClass,
            [CanBeNull] FixedList<ExpressionSyntax> baseTypes,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<ExpressionSyntax> invariants,
            [NotNull] FixedList<EnumVariantSyntax> variants,
            [NotNull] FixedList<IMemberDeclarationSyntax> members)
            : base(file, nameSpan, fullName, genericParameters, members)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            BaseClass = baseClass;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
            Variants = variants;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
