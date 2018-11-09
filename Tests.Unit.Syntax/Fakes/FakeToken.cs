using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes
{
    public class FakeToken : IToken
    {
        public TextSpan Span { get; }

        [CanBeNull]
        public readonly object FakeValue;

        public FakeToken(TextSpan span, [CanBeNull] object fakeValue)
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
