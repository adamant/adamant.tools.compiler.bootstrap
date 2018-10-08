using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public static class ExpectFakeExtension
    {
        public static Token ExpectFake(this ITokenStream tokens)
        {
            Assert.Equal(FakeToken.Kind, tokens.Current?.Kind);

            var token = tokens.Current.Value;
            tokens.Next();
            return token;
        }
    }
}
