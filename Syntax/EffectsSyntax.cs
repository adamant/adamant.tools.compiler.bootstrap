using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EffectsSyntax : SyntaxNode
    {
        [NotNull] public IMayKeywordToken MayKeyword { get; }
        [NotNull] public SeparatedListSyntax<EffectSyntax> AllowedEffects { get; }
        [NotNull] public INoKeywordToken NoKeyword { get; }
        [NotNull] public SeparatedListSyntax<EffectSyntax> DisallowedEffects { get; }

        public EffectsSyntax(
            [NotNull] IMayKeywordToken mayKeyword,
            [NotNull] SeparatedListSyntax<EffectSyntax> allowedEffects,
            [NotNull] INoKeywordToken noKeyword,
            [NotNull] SeparatedListSyntax<EffectSyntax> disallowedEffects)
        {
            Requires.NotNull(nameof(mayKeyword), mayKeyword);
            Requires.NotNull(nameof(allowedEffects), allowedEffects);
            Requires.NotNull(nameof(noKeyword), noKeyword);
            Requires.NotNull(nameof(disallowedEffects), disallowedEffects);
            MayKeyword = mayKeyword;
            AllowedEffects = allowedEffects;
            NoKeyword = noKeyword;
            DisallowedEffects = disallowedEffects;
        }
    }
}
