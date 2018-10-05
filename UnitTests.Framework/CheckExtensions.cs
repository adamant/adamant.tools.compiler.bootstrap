using FsCheck;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.UnitTests.Framework
{
    public static class CheckExtensions
    {
        public static void QuickCheckThrow(
            this Property property,
            ITestOutputHelper testOutput)
        {
            Check.One(Config.QuickThrowOnFailure.WithTestOutput(testOutput), property);
        }

        public static void QuickCheckThrow(
            this Property property,
            int maxTest,
            ITestOutputHelper testOutput)
        {
            Check.One(Config.QuickThrowOnFailure.WithTestOutput(testOutput).WithMaxTest(maxTest), property);
        }
    }
}
