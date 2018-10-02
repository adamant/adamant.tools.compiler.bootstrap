using System.Diagnostics;
using System.IO;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.API;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class RunTests : IClassFixture<RuntimeLibraryFixture>
    {
        private readonly ITestOutputHelper output;

        public RunTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [Category("Run")]
        [MemberData(nameof(GetAllRunTestCases))]
        public void Runs(RunTestCase testCase)
        {
            var testsDirectory = LangTestsDirectory.Get();
            var runTestsDirectory = Path.Combine(testsDirectory, "run");
            var codePath = new CodePath(testCase.RelativeCodePath);
            var code = new CodeText(testCase.Code);
            var codeFile = new CodeFile(codePath, code);
            var cCodeFile = Path.Combine(runTestsDirectory, Path.ChangeExtension(testCase.RelativeCodePath, "c"));
            CompileAdamantToC(codeFile, cCodeFile);
            var executable = CompileCToExecutable(cCodeFile);
            AssertRuns(testCase, executable);
        }

        private static void CompileAdamantToC(CodeFile code, string outputPath)
        {
            var package = new AdamantCompiler().CompilePackage(code.Yield());
            // TODO Assert.Empty(package.GetDiagnostics().Where(d=>d.IsError);
            var cCode = new CEmitter().Emit(package);
            File.WriteAllText(outputPath, cCode, Encoding.ASCII);
        }

        private string CompileCToExecutable(string cCodeFile)
        {
            var options = "-std=c11 -fsanitize=undefined -fsanitize=integer -fsanitize=nullability -Wall -Wno-incompatible-pointer-types";
            // Next thing is needed for windows
            options += " -Xclang -flto-visibility-public-std";
            var sources = string.Join(" ", cCodeFile, RuntimeLibraryFixture.GetRuntimeLibraryPath());
            var outputPath = Path.ChangeExtension(cCodeFile, "exe");
            var arguments = $"{sources} -o {outputPath} --include-directory {RuntimeLibraryFixture.GetRuntimeDirectory()} {options}";
            output.WriteLine("clang arguments:");
            output.WriteLine(arguments);
            var startInfo = new ProcessStartInfo("clang", arguments)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            var process = Process.Start(startInfo);
            process.WaitForExit();
            output.WriteLine("clang stdout:");
            output.WriteLine(process.StandardOutput.ReadToEnd());
            output.WriteLine("clang stderr:");
            output.WriteLine(process.StandardError.ReadToEnd());
            Assert.True(process.ExitCode == 0, $"clang exited with {process.ExitCode}");
            return outputPath;
        }

        private static void AssertRuns(RunTestCase testCase, string executable)
        {
            var startInfo = new ProcessStartInfo(executable)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.Equal(testCase.Stdout ?? "", process.StandardOutput.ReadToEnd());
            Assert.Equal(testCase.Stderr ?? "", process.StandardError.ReadToEnd());
            Assert.Equal(testCase.ExitCode, process.ExitCode);
        }

        [Fact]
        [Category("Run")]
        public void CanGetAllRunTestCases()
        {
            Assert.NotEmpty(GetAllRunTestCases());
        }

        public static TheoryData<RunTestCase> GetAllRunTestCases()
        {
            var testCases = new TheoryData<RunTestCase>();
            var testsDirectory = LangTestsDirectory.Get();
            var runTestsDirectory = Path.Combine(testsDirectory, "run");
            foreach (var fullCodePath in Directory.EnumerateFiles(runTestsDirectory, "*.ad", SearchOption.AllDirectories))
            {
                var relativeCodePath = Path.GetRelativePath(runTestsDirectory, fullCodePath);
                testCases.Add(new RunTestCase(fullCodePath, relativeCodePath));
            }
            return testCases;
        }
    }
}
