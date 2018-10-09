using FsCheck;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Tests
{
    public static class TestHelpers
    {
        [NotNull]
        public static CodeFile ToFakeCodeFile([NotNull] this string text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text));
        }

        [NotNull]
        public static CodeFile ToFakeCodeFile([NotNull] this NonNull<string> text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text.Get));
        }
    }
}
