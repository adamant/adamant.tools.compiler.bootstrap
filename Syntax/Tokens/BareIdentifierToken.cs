using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class BareIdentifierToken : IdentifierToken
    {
        public BareIdentifierToken(TextSpan span, string value)
            : base(span, value)
        {
        }
    }
}
