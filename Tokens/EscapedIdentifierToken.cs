using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class EscapedIdentifierToken : IdentifierToken, IEscapedIdentifierToken
    {
        public EscapedIdentifierToken(TextSpan span, [NotNull] string value)
            : base(span, value)
        {
        }
    }

    public static partial class TokenFactory
    {
        [NotNull]
        public static IEscapedIdentifierToken EscapedIdentifier(TextSpan span, [NotNull] string value)
        {
            return new EscapedIdentifierToken(span, value);
        }
    }
}
