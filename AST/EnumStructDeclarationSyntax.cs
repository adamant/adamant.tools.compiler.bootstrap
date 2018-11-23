using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class EnumStructDeclarationSyntax : TypeDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [CanBeNull] public FixedList<ExpressionSyntax> BaseTypes { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public FixedList<ExpressionSyntax> Invariants { get; }
        [NotNull] public FixedList<EnumVariantSyntax> Variants { get; }
        [NotNull] public FixedList<IMemberDeclarationSyntax> Members { get; }

        public EnumStructDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] string name,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [CanBeNull] FixedList<ExpressionSyntax> baseTypes,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<ExpressionSyntax> invariants,
            [NotNull] FixedList<EnumVariantSyntax> variants,
            [NotNull] FixedList<IMemberDeclarationSyntax> members)
            : base(file, nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            Name = new SimpleName(name);
            GenericParameters = genericParameters;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
            Variants = variants;
            Members = members;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
