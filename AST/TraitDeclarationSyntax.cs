using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class TraitDeclarationSyntax : TypeDeclarationSyntax
    {
        public FixedList<AttributeSyntax> Attributes { get; }
        public FixedList<IModiferToken> Modifiers { get; }
        public FixedList<ExpressionSyntax> BaseTypes { get; }
        public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        public FixedList<ExpressionSyntax> Invariants { get; }

        public TraitDeclarationSyntax(
            CodeFile file,
            FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<GenericParameterSyntax> genericParameters,
            FixedList<ExpressionSyntax> baseTypes,
            FixedList<GenericConstraintSyntax> genericConstraints,
            FixedList<ExpressionSyntax> invariants,
            FixedList<MemberDeclarationSyntax> members)
            : base(file, nameSpan, fullName, genericParameters, members)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
