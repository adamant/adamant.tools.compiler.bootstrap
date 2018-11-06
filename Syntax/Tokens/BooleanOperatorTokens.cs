using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class NotKeywordToken : KeywordOperatorToken
    {
        public NotKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class AndKeywordToken : KeywordOperatorToken
    {
        public AndKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class OrKeywordToken : KeywordOperatorToken
    {
        public OrKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class XorKeywordToken : KeywordOperatorToken
    {
        public XorKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
