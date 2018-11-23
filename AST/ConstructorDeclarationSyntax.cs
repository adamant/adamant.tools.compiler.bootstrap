using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ConstructorDeclarationSyntax : FunctionDeclarationSyntax
    {
        [CanBeNull] public SimpleName Name { get; }

        public ConstructorDeclarationSyntax(
            [NotNull] CodeFile file,
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
            : base(file, modifiers, nameSpan, genericParameters, parameters, genericConstraints,
                mayEffects, noEffects, requires, ensures, body)
        {
            if (name != null) Name = new SimpleName(name);
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
