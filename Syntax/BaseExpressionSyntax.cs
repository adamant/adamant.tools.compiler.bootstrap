using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BaseExpressionSyntax : InstanceExpressionSyntax
    {
        [NotNull] public BaseKeywordToken Token { get; }

        public BaseExpressionSyntax([NotNull] BaseKeywordToken token)
            : base(token.Span)
        {
            Token = token;
        }
    }
}
