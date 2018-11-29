using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GetterDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public Name PropertyName { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }

        public GetterDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] Name propertyName,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, null, parameters,
                FixedList<GenericConstraintSyntax>.Empty, mayEffects, noEffects, requires, ensures, body)
        {
            Attributes = attributes;
            PropertyName = propertyName;
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
