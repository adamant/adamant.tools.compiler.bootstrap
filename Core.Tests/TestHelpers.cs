using FsCheck;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Tests
{
    public static class TestHelpers
    {
        public static CodeFile ToFakeCodeFile(this string text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text));
        }

        public static CodeFile ToFakeCodeFile(this NonNull<string> text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text.Get));
        }
    }
}
