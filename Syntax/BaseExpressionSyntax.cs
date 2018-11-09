using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BaseExpressionSyntax : InstanceExpressionSyntax
    {
        [NotNull] public IBaseKeywordToken Token { get; }

        public BaseExpressionSyntax([NotNull] IBaseKeywordToken token)
            : base(token.Span)
        {
            Token = token;
        }
    }
}
