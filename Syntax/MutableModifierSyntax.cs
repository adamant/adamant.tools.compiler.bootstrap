using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MutableModifierSyntax : ModifierSyntax
    {
        [NotNull] public MutableKeywordToken MutableKeyword { get; }

        public MutableModifierSyntax([NotNull] MutableKeywordToken mutableKeyword)
        {
            Requires.NotNull(nameof(mutableKeyword), mutableKeyword);
            MutableKeyword = mutableKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return MutableKeyword;
        }
    }
}
