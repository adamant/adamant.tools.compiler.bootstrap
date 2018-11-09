using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IReturnKeywordTokenPlace ReturnKeyword { get; }
        [CanBeNull] public ExpressionSyntax ReturnValue { get; }

        public ReturnExpressionSyntax(
            [NotNull] IReturnKeywordTokenPlace returnKeyword,
            [CanBeNull] ExpressionSyntax returnValue)
            : base(returnValue == null ? returnKeyword.Span : TextSpan.Covering(returnKeyword.Span, returnValue.Span))
        {
            Requires.NotNull(nameof(returnKeyword), returnKeyword);
            ReturnKeyword = returnKeyword;
            ReturnValue = returnValue;
        }
    }
}
