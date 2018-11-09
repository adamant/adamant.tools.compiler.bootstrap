using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ExtendModifierSyntax : ModifierSyntax
    {
        [NotNull] public IExtendKeywordToken ExtendKeyword { get; }

        public ExtendModifierSyntax([NotNull] IExtendKeywordToken extendKeyword)
        {
            Requires.NotNull(nameof(extendKeyword), extendKeyword);
            ExtendKeyword = extendKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return ExtendKeyword;
        }
    }
}
