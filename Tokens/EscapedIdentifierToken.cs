using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class EscapedIdentifierToken : IdentifierToken, IEscapedIdentifierToken
    {
        public EscapedIdentifierToken(TextSpan span, string value)
            : base(span, value)
        {
        }
    }

    public static partial class TokenFactory
    {

        public static IEscapedIdentifierToken EscapedIdentifier(TextSpan span, string value)
        {
            return new EscapedIdentifierToken(span, value);
        }
    }
}
