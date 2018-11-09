using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MoveModifierSyntax : ModifierSyntax
    {
        [NotNull] public IMoveKeywordToken MoveKeyword { get; }

        public MoveModifierSyntax([NotNull] IMoveKeywordToken moveKeyword)
        {
            Requires.NotNull(nameof(moveKeyword), moveKeyword);
            MoveKeyword = moveKeyword;
        }

        public override IEnumerable<ITokenPlace> Tokens()
        {
            yield return MoveKeyword;
        }
    }
}
