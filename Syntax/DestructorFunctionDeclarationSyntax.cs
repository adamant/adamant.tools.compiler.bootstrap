using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class DestructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IDeleteKeywordToken DeleteKeyword { get; }
        public override IIdentifierTokenPlace Name => throw new System.NotImplementedException();

        public DestructorFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IDeleteKeywordToken deleteKeyword,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [NotNull] BlockSyntax body)
            : base(deleteKeyword.Span, modifiers, parameters,
                mayEffects, noEffects, requires, ensures, body)
        {
            DeleteKeyword = deleteKeyword;
        }
    }
}
