using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class RefModifierSyntax : ModifierSyntax
    {
        [NotNull] public IRefKeywordToken RefKeyword { get; }

        public RefModifierSyntax([NotNull] IRefKeywordToken refKeyword)
        {
            Requires.NotNull(nameof(refKeyword), refKeyword);
            RefKeyword = refKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return RefKeyword;
        }
    }
}
