using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.UnitTests.Helpers
{
    public static class LangTestsDirectory
    {
        public static string Get()
        {
            return Path.Combine(SolutionDirectory.Get(), "tests");
        }
    }
}
