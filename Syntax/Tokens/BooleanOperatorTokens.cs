using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class NotKeywordToken : BooleanOperatorToken
    {
        public NotKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class AndKeywordToken : BooleanOperatorToken
    {
        public AndKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class OrKeywordToken : BooleanOperatorToken
    {
        public OrKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public class XorKeywordToken : BooleanOperatorToken
    {
        public XorKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
