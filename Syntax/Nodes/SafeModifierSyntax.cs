using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SafeModifierSyntax : ModifierSyntax
    {
        [NotNull] public SafeKeywordToken SafeKeyword { get; }

        public SafeModifierSyntax([NotNull] SafeKeywordToken safeKeyword)
        {
            Requires.NotNull(nameof(safeKeyword), safeKeyword);
            SafeKeyword = safeKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return SafeKeyword;
        }
    }
}
