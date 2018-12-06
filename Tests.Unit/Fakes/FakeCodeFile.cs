using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    public static class FakeCodeFile
    {

        public static CodeFile For(string text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text));
        }
    }
}
