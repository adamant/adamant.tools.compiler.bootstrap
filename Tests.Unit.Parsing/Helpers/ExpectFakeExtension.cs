using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    public static class ExpectFakeExtension
    {
        [CanBeNull]
        public static FakeToken ExpectFake([NotNull] this ITokenIterator tokens)
        {
            if (tokens.Current is FakeToken token)
            {
                tokens.Next();
                return token;
            }

            return null;
        }
    }
}
