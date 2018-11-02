using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class UnsafeModifierSyntax : ModifierSyntax
    {
        [NotNull] public UnsafeKeywordToken UnsafeKeyword { get; }

        public UnsafeModifierSyntax([NotNull] UnsafeKeywordToken unsafeKeyword)
        {
            Requires.NotNull(nameof(unsafeKeyword), unsafeKeyword);
            UnsafeKeyword = unsafeKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return UnsafeKeyword;
        }
    }
}
