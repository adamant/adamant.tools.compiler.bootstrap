using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Conformance.Helpers
{
    public class CompilerOutputAdapter : ICompilerOutput
    {
        [NotNull] private readonly ITestOutputHelper testOutput;

        public CompilerOutputAdapter([NotNull]ITestOutputHelper testOutput)
        {
            Requires.NotNull(nameof(testOutput), testOutput);
            this.testOutput = testOutput;
        }

        public void WriteLine([CanBeNull] string message)
        {
            if (message != null)
                testOutput.WriteLine(message);
        }
    }
}
