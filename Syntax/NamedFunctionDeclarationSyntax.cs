using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [CanBeNull] public ExpressionSyntax ReturnType { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }

        public NamedFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierToken name,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters, // For now we will not support pure meta functions
            [CanBeNull] ExpressionSyntax returnType,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(modifiers, name.Span, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            Name = name;
            GenericParameters = genericParameters;
            ReturnType = returnType;
            GenericConstraints = genericConstraints;
        }
    }
}
