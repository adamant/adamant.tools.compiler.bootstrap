using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class DestructorDeclarationSyntax : FunctionDeclarationSyntax
    {
        public DestructorDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<IModiferToken> modifiers,
            TextSpan nameSpan,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [NotNull] BlockSyntax body)
            : base(file, modifiers, nameSpan, null, parameters,
                FixedList<GenericConstraintSyntax>.Empty, mayEffects, noEffects, requires, ensures, body)
        {
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
