using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeUsingDirectiveParser : IParser<UsingDirectiveSyntax>
    {
        public UsingDirectiveSyntax Parse(ITokenStream tokens)
        {
            var usingKeyword = tokens.ExpectSimple(TokenKind.UsingKeyword);
            Assert.Equal(TokenKind.UsingKeyword, usingKeyword.Kind);

            var fakeToken = tokens.ExpectFake();
            return (UsingDirectiveSyntax)fakeToken.Value;
        }
    }
}
