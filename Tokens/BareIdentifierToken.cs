using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class BareIdentifierToken : IdentifierToken, IBareIdentifierToken
    {
        public BareIdentifierToken(TextSpan span, string value)
            : base(span, value)
        {
        }
    }

    public static partial class TokenFactory
    {

        public static IIdentifierToken BareIdentifier(TextSpan span, string value)
        {
            return new BareIdentifierToken(span, value);
        }
    }
}
