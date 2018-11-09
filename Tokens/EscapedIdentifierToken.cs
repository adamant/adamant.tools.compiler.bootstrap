using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public class EscapedIdentifierToken : IdentifierToken
    {
        public EscapedIdentifierToken(TextSpan span, string value)
            : base(span, value)
        {
        }
    }
}
