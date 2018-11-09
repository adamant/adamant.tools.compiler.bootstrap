using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ExtendModifierSyntax : ModifierSyntax
    {
        [NotNull] public ExtendKeywordToken ExtendKeyword { get; }

        public ExtendModifierSyntax([NotNull] ExtendKeywordToken extendKeyword)
        {
            Requires.NotNull(nameof(extendKeyword), extendKeyword);
            ExtendKeyword = extendKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return ExtendKeyword;
        }
    }
}
