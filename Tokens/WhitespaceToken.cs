using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public class WhitespaceToken : TriviaToken
    {
        public WhitespaceToken(TextSpan span)
            : base(span)
        {
        }
    }
}
