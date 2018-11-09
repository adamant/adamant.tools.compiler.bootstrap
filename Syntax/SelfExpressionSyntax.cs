using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfExpressionSyntax : InstanceExpressionSyntax
    {
        [NotNull] public ISelfKeywordToken Token { get; }

        public SelfExpressionSyntax([NotNull] ISelfKeywordToken token)
            : base(token.Span)
        {
            Token = token;
        }
    }
}
