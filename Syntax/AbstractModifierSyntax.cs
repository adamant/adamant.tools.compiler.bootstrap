using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AbstractModifierSyntax : ModifierSyntax
    {
        [NotNull] public IAbstractKeywordToken AbstractKeyword { get; }

        public AbstractModifierSyntax([NotNull] IAbstractKeywordToken abstractKeyword)
        {
            Requires.NotNull(nameof(abstractKeyword), abstractKeyword);
            AbstractKeyword = abstractKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return AbstractKeyword;
        }
    }
}
