using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes
{
    public static class FakeCodeFile
    {
        [NotNull]
        public static CodeFile For([NotNull] string text)
        {
            return new CodeFile(FakeCodeReference.Instance, new CodeText(text));
        }
    }
}
