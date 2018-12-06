using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes
{
    public static class FakeParseContext
    {

        public static ParseContext For(string text)
        {
            return new ParseContext(FakeCodeFile.For(text), new Diagnostics());
        }
    }
}
