using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfTypeExpressionSyntax : TypeSyntax
    {
        [NotNull] public ISelfTypeKeywordToken SelfTypeKeyword { get; }

        public SelfTypeExpressionSyntax([NotNull] ISelfTypeKeywordToken selfTypeKeyword)
            : base(selfTypeKeyword.Span)
        {
            Requires.NotNull(nameof(selfTypeKeyword), selfTypeKeyword);
            SelfTypeKeyword = selfTypeKeyword;
        }
    }
}
