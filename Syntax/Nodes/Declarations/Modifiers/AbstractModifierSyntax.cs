using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers
{
    public class AbstractModifierSyntax : ModifierSyntax
    {
        [NotNull] public AbstractKeywordToken AbstractKeyword { get; }

        public AbstractModifierSyntax([NotNull] AbstractKeywordToken abstractKeyword)
        {
            Requires.NotNull(nameof(abstractKeyword), abstractKeyword);
            AbstractKeyword = abstractKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return AbstractKeyword;
        }
    }
}