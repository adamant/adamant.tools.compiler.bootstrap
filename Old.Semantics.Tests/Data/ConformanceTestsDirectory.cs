using System.IO;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Tests.Data
{
    public static class ConformanceTestsDirectory
    {
        public static string Get()
        {
            return Path.Combine(SolutionDirectory.Get(), "tests");
        }
    }
}
