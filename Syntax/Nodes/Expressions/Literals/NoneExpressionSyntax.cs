using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public class NoneExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public NoneKeywordToken NoneKeyword { get; }

        public NoneExpressionSyntax([NotNull] NoneKeywordToken noneKeyword)
            : base(noneKeyword.Span)
        {
            Requires.NotNull(nameof(noneKeyword), noneKeyword);
            NoneKeyword = noneKeyword;
        }
    }
}
