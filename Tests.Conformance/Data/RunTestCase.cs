using System;
using System.IO;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Conformance.Data
{
    public class RunTestCase : TestCase
    {
        private readonly Lazy<string> stdout;
        public string Stdout => stdout.Value;
        private readonly Lazy<string> stderr;
        public string Stderr => stderr.Value;
        public int ExitCode
        {
            get
            {
                var exitCode = Regex.Match(Code, @"// exit code: (?<exitCode>\d+)").Groups["exitCode"].Value;
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

        private string GetStdout()
        {
            var stdoutFile = Path.ChangeExtension(FullCodePath, "stdout");
            return File.Exists(stdoutFile) ? File.ReadAllText(stdoutFile) : null;
        }

        private string GetStderr()
        {
            var stderrFile = Path.ChangeExtension(FullCodePath, "stderr");
            return File.Exists(stderrFile) ? File.ReadAllText(stderrFile) : null;
        }
    }
}
