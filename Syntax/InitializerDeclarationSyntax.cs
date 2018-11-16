using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class InitializerDeclarationSyntax : FunctionDeclarationSyntax
    {
        [CanBeNull] public SimpleName Name { get; }
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }

        public InitializerDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [CanBeNull] string name,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [NotNull] BlockSyntax body)
            : base(modifiers,
                nameSpan, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            if (name != null) Name = new SimpleName(name);
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
