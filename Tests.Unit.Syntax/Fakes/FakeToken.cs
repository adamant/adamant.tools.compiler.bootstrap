using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes
{
    public class FakeToken : Token
    {
        [CanBeNull]
        public readonly object FakeValue;

        public FakeToken(TextSpan span, [CanBeNull] object fakeValue)
            : base(span)
        {
            FakeValue = fakeValue;
        }
    }
}
