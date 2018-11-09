using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public class CommentToken : TriviaToken
    {
        public CommentToken(TextSpan span)
            : base(span)
        {
        }
    }
}
