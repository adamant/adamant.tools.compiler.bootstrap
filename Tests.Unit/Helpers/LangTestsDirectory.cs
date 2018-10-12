using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers
{
    public static class LangTestsDirectory
    {
        public static string Get()
        {
            return Path.Combine(SolutionDirectory.Get(), "tests");
        }
    }
}
