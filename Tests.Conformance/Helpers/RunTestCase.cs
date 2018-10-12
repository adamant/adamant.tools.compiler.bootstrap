using System;
using System.IO;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Conformance.Helpers
{
    // TODO move standard in and standard error expected values into comments in the code file
    public class RunTestCase : TestCase
    {
        [NotNull] private static readonly Regex ExitCodePattern = new Regex(@"// exit code: (?<exitCode>\d+)", RegexOptions.Compiled);

        [NotNull] private readonly Lazy<string> stdout;
        [CanBeNull] public string Stdout => stdout.Value;
        [NotNull] private readonly Lazy<string> stderr;
        [CanBeNull] public string Stderr => stderr.Value;
        public int ExitCode
        {
            get
            {
                var exitCode = ExitCodePattern.Match(Code).Groups["exitCode"]?.Value;
                return int.Parse(exitCode);
            }
        }

        [Obsolete("Required by IXunitSerializable", true)]
        public RunTestCase()
        {
            stdout = new Lazy<string>(GetStdout);
            stderr = new Lazy<string>(GetStderr);
        }

        public RunTestCase(string fullCodePath, string relativeCodePath)
            : base(fullCodePath, relativeCodePath)
        {
            stdout = new Lazy<string>(GetStdout);
            stderr = new Lazy<string>(GetStderr);
        }

        [CanBeNull]
        private string GetStdout()
        {
            var stdoutFile = Path.ChangeExtension(FullCodePath, "stdout");
            return File.Exists(stdoutFile) ? File.ReadAllText(stdoutFile) : null;
        }

        [CanBeNull]
        private string GetStderr()
        {
            var stderrFile = Path.ChangeExtension(FullCodePath, "stderr");
            return File.Exists(stderrFile) ? File.ReadAllText(stderrFile) : null;
        }
    }
}
