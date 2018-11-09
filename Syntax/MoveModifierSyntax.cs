using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MoveModifierSyntax : ModifierSyntax
    {
        [NotNull] public MoveKeywordToken MoveKeyword { get; }

        public MoveModifierSyntax([NotNull] MoveKeywordToken moveKeyword)
        {
            Requires.NotNull(nameof(moveKeyword), moveKeyword);
            MoveKeyword = moveKeyword;
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return MoveKeyword;
        }
    }
}
