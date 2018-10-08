using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeToken
    {
        public const TokenKind Kind = (TokenKind)sbyte.MinValue;
        public static readonly FakeToken Instance = new FakeToken();

        private FakeToken() { }

        public static implicit operator SimpleToken(FakeToken token)
        {
            return new SimpleToken(Kind, new TextSpan(0, 0));
        }
    }
}
