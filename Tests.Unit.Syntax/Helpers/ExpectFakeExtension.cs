using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers
{
    public static class ExpectFakeExtension
    {
        [CanBeNull]
        public static FakeToken ExpectFake([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is FakeToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }
    }
}
