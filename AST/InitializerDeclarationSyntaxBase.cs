using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class InitializerDeclarationSyntaxBase : FunctionDeclarationSyntax
    {
        [NotNull] public TypePromise SelfParameterType { get; } = new TypePromise();

        protected InitializerDeclarationSyntaxBase(
            [NotNull] CodeFile file,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [NotNull] BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, genericParameters, parameters, genericConstraints,
                mayEffects, noEffects, requires, ensures, body)
        {
        }

        public override string ToString()
        {
            if (GenericParameters != null)
                return $"{FullName}[{string.Join(", ", GenericParameters)}]({string.Join(", ", Parameters)})";

            return $"{FullName}({string.Join(", ", Parameters)})";
        }
    }
}
