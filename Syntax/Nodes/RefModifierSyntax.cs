using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class RefModifierSyntax : ModifierSyntax
    {
        [NotNull] public RefKeywordToken RefKeyword { get; }

        public RefModifierSyntax([NotNull] RefKeywordToken refKeyword)
        {
            Requires.NotNull(nameof(refKeyword), refKeyword);
            RefKeyword = refKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return RefKeyword;
        }
    }
}
