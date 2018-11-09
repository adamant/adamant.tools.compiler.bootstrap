using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UnsafeModifierSyntax : ModifierSyntax
    {
        [NotNull] public IUnsafeKeywordToken UnsafeKeyword { get; }

        public UnsafeModifierSyntax([NotNull] IUnsafeKeywordToken unsafeKeyword)
        {
            Requires.NotNull(nameof(unsafeKeyword), unsafeKeyword);
            UnsafeKeyword = unsafeKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return UnsafeKeyword;
        }
    }
}
