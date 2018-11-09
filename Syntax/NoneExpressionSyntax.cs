using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NoneExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public INoneKeywordToken NoneKeyword { get; }

        public NoneExpressionSyntax([NotNull] INoneKeywordToken noneKeyword)
            : base(noneKeyword.Span)
        {
            Requires.NotNull(nameof(noneKeyword), noneKeyword);
            NoneKeyword = noneKeyword;
        }
    }
}
