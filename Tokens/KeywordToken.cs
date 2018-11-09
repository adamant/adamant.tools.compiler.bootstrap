using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IKeywordToken : IToken { }
    public abstract class KeywordToken : Token, IKeywordToken
    {
        protected KeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
