using FsCheck;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.UnitTests.Framework
{
    public static class ConfigExtensions
    {
        // Base copy of config:
        // new Config(c.MaxTest, c.MaxFail, c.Replay, c.Name, c.StartSize, c.EndSize, c.QuietOnSuccess, c.Every, c.EveryShrink, c.Arbitrary, c.Runner);

        public static Config WithMaxTest(this Config c, int maxTest)
        {
            return new Config(maxTest, c.MaxFail, c.Replay, c.Name, c.StartSize, c.EndSize, c.QuietOnSuccess, c.Every, c.EveryShrink, c.Arbitrary, c.Runner);
        }

        public static Config WithRunner(this Config c, IRunner runner)
        {
            return new Config(c.MaxTest, c.MaxFail, c.Replay, c.Name, c.StartSize, c.EndSize, c.QuietOnSuccess, c.Every, c.EveryShrink, c.Arbitrary, runner);
        }

        public static Config WithTestOutput(this Config c, ITestOutputHelper testOutput)
        {
            return c.WithRunner(new FsCheckRunner(testOutput));
        }
    }
}
