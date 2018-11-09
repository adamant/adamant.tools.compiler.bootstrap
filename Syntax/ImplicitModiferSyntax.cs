using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ImplicitModiferSyntax : ModifierSyntax
    {
        [NotNull] public ImplicitKeywordToken ImplicitKeyword { get; }

        public ImplicitModiferSyntax([NotNull] ImplicitKeywordToken implicitKeyword)
        {
            Requires.NotNull(nameof(implicitKeyword), implicitKeyword);
            ImplicitKeyword = implicitKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return ImplicitKeyword;
        }
    }
}
