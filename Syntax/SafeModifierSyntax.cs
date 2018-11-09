using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SafeModifierSyntax : ModifierSyntax
    {
        [NotNull] public ISafeKeywordToken SafeKeyword { get; }

        public SafeModifierSyntax([NotNull] ISafeKeywordToken safeKeyword)
        {
            Requires.NotNull(nameof(safeKeyword), safeKeyword);
            SafeKeyword = safeKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return SafeKeyword;
        }
    }
}
