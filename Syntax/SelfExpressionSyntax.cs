using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfExpressionSyntax : InstanceExpressionSyntax
    {
        [NotNull] public SelfKeywordToken Token { get; }

        public SelfExpressionSyntax([NotNull] SelfKeywordToken token)
            : base(token.Span)
        {
            Token = token;
        }
    }
}
