using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public partial interface IAsKeywordToken : IKeywordToken { }
    public class AsKeywordToken : KeywordOperatorToken, IAsKeywordToken
    {
        public AsKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
