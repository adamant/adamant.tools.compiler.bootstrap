using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class BareIdentifierToken : IdentifierToken, IBareIdentifierToken
    {
        public BareIdentifierToken(TextSpan span, [NotNull] string value)
            : base(span, value)
        {
        }
    }

    public static partial class TokenFactory
    {
        [NotNull]
        public static IIdentifierToken BareIdentifier(TextSpan span, [NotNull] string value)
        {
            return new BareIdentifierToken(span, value);
        }
    }
}
