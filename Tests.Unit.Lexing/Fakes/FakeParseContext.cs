using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes
{
    public static class FakeParseContext
    {
        [NotNull]
        public static ParseContext For([NotNull] string text)
        {
            return new ParseContext(FakeCodeFile.For(text), new Diagnostics());
        }
    }
}
