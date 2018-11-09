using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EffectsSyntax : NonTerminal
    {
        [NotNull] public IMayKeywordTokenPlace MayKeyword { get; }
        [NotNull] public SeparatedListSyntax<EffectSyntax> AllowedEffects { get; }
        [NotNull] public INoKeywordTokenPlace NoKeyword { get; }
        [NotNull] public SeparatedListSyntax<EffectSyntax> DisallowedEffects { get; }

        public EffectsSyntax(
            [NotNull] IMayKeywordTokenPlace mayKeyword,
            [NotNull] SeparatedListSyntax<EffectSyntax> allowedEffects,
            [NotNull] INoKeywordTokenPlace noKeyword,
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
