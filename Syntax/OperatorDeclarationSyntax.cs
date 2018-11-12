using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class OperatorDeclarationSyntax : FunctionDeclarationSyntax
    {
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [CanBeNull] public ExpressionSyntax ReturnType { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }

        public OperatorDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [CanBeNull] ExpressionSyntax returnType,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(modifiers, nameSpan, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            GenericParameters = genericParameters;
            ReturnType = returnType;
            GenericConstraints = genericConstraints;
        }
    }
}
