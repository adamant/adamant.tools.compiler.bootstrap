using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ImplicitModiferSyntax : ModifierSyntax
    {
        [NotNull] public IImplicitKeywordToken ImplicitKeyword { get; }

        public ImplicitModiferSyntax([NotNull] IImplicitKeywordToken implicitKeyword)
        {
            Requires.NotNull(nameof(implicitKeyword), implicitKeyword);
            ImplicitKeyword = implicitKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return ImplicitKeyword;
        }
    }
}
