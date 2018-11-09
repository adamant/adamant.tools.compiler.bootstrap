using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MutableModifierSyntax : ModifierSyntax
    {
        [NotNull] public IMutableKeywordToken MutableKeyword { get; }

        public MutableModifierSyntax([NotNull] IMutableKeywordToken mutableKeyword)
        {
            Requires.NotNull(nameof(mutableKeyword), mutableKeyword);
            MutableKeyword = mutableKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return MutableKeyword;
        }
    }
}
