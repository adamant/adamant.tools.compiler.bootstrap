using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes
{
    public class FakeToken : IToken
    {
        public TextSpan Span { get; }

        public readonly object FakeValue;

        public FakeToken(TextSpan span, object fakeValue)
        {
            Span = span;
            FakeValue = fakeValue;
        }

        public string Text(CodeText code)
        {
            throw new System.NotImplementedException();
        }
    }
}
