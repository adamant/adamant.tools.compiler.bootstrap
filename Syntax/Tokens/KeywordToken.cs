using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public abstract class KeywordToken : Token
    {
        protected KeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
