using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public override IIdentifierTokenPlace Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }

        public NamedFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [CanBeNull] FixedList<ParameterSyntax> parameters,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(name.Span, modifiers, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            Name = name;
            GenericParameters = genericParameters;
            ReturnTypeExpression = returnTypeExpression;
            GenericConstraints = genericConstraints;
        }
    }
}
