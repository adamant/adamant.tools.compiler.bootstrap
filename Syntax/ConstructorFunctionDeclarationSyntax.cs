using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ConstructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [CanBeNull] public override IIdentifierTokenPlace Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }

        public ConstructorFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [CanBeNull] IIdentifierToken name,
            TextSpan span,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [NotNull] BlockSyntax body)
            : base(span, modifiers, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            Name = name;
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
        }
    }
}
