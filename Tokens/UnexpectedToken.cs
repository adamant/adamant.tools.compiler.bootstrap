using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public class UnexpectedToken : TriviaToken
    {
        public UnexpectedToken(TextSpan span)
            : base(span)
        {
        }
    }
}
