using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.API;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Tests.Conformance.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Conformance
{
    [IntegrationTest]
    [Category("Conformance")]
    [Category("Compile")]
    public class ConformanceTests : IClassFixture<RuntimeLibraryFixture>
    {
        [NotNull] private readonly ITestOutputHelper testOutput;

        public ConformanceTests([NotNull] ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public void Can_load_all_conformance_test_cases()
        {
            Assert.NotEmpty(GetConformanceTestCases());
        }

        [NotNull] private static readonly Regex ExitCodePattern = new Regex(@"// exit code: (?<exitCode>\d+)", RegexOptions.Compiled);
        [NotNull] private const string ExpectedOutputFormat = "/\\* {0}:\n(?<output>\\**[^/])*\\*/";
        [NotNull] private static readonly Regex ErrorPattern = new Regex(@"// ERROR([ \t].*)?", RegexOptions.Compiled | RegexOptions.Multiline);

        [Theory]
        [MemberData(nameof(GetConformanceTestCases))]
        public void Test_cases([NotNull] TestCase testCase)
        {
            // Setup
            var codeFile = CodeFile.Load(testCase.FullCodePath);
            var code = codeFile.Code.Text;
            var compiler = new AdamantCompiler();
            var references = new Dictionary<string, Package>();

            // Analyze
            var package = compiler.CompilePackage("testPackage", codeFile.Yield(), references);

            // Check for compiler errors
            var expectedCompileErrorLines = ExpectedCompileErrorLines(codeFile, code);
            Assert.NotNull(package.Diagnostics);
            var diagnostics = package.Diagnostics;

            if (diagnostics.Any())
            {
                testOutput.WriteLine("Compiler Errors:");
                foreach (var diagnostic in diagnostics)
                {
                    testOutput.WriteLine($"{testCase.RelativeCodePath}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                    testOutput.WriteLine(diagnostic.Message);
                }
            }

            if (expectedCompileErrorLines.Any())
            {
                foreach (var expectedCompileErrorLine in expectedCompileErrorLines)
                {
                    // Assert a single error on the given line
                    Assert.Single(diagnostics.Where(d =>
                        d.StartPosition.Line == expectedCompileErrorLine
                        && d.Level >= DiagnosticLevel.CompilationError));
                }
                // Done with test
                return;
            }
            else
                Assert.Empty(diagnostics);

            // Emit Code
            var codePath = Path.ChangeExtension(testCase.FullCodePath, "c").AssertNotNull();
            EmitCode(package, codePath);

            // Compile Code to Executable
            var exePath = CompileToExecutable(codePath);

            // Execute and check results
            var process = Execute(exePath);
            process.WaitForExit();
            Assert.Equal(ExpectedOutput(code, "stdout"), process.StandardOutput.ReadToEnd());
            Assert.Equal(ExpectedOutput(code, "stderr"), process.StandardError.ReadToEnd());
            Assert.Equal(ExpectedExitCode(code), process.ExitCode);
        }

        [NotNull]
        private static List<int> ExpectedCompileErrorLines([NotNull] CodeFile codeFile, [NotNull] string code)
        {
            return ErrorPattern.Matches(code).AssertItemNotNull()
                .Select(match => codeFile.Code.Lines.LineIndexContainingOffset(match.Index) + 1)
                .ToList();
        }

        private static void EmitCode(
            [NotNull] Package package,
            [NotNull] string path)
        {
            var codeEmitter = new CodeEmitter();
            codeEmitter.Emit(package);
            File.WriteAllText(path, codeEmitter.GetEmittedCode(), Encoding.UTF8);
        }

        [NotNull]
        private string CompileToExecutable([NotNull] string codePath)
        {
            // used to have: -Wno-incompatible-pointer-types
            var options = "-std=c11 -fsanitize=undefined -fsanitize=integer -fsanitize=nullability -Wall  -Wno-unused-label";
            // Next thing is needed for windows
            options += " -Xclang -flto-visibility-public-std";
            var sources = string.Join(" ", codePath, RuntimeLibraryFixture.GetRuntimeLibraryPath());
            var outputPath = Path.ChangeExtension(codePath, "exe").AssertNotNull();
            var arguments = $"{sources} -o {outputPath} --include-directory {RuntimeLibraryFixture.GetRuntimeDirectory()} {options}";
            testOutput.WriteLine("clang arguments:");
            testOutput.WriteLine(arguments);
            var startInfo = new ProcessStartInfo("clang", arguments)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            var process = Process.Start(startInfo).AssertNotNull();
            process.WaitForExit();
            testOutput.WriteLine("clang stdout:");
            testOutput.WriteLine(process.StandardOutput.ReadToEnd());
            testOutput.WriteLine("clang stderr:");
            testOutput.WriteLine(process.StandardError.ReadToEnd());
            Assert.True(process.ExitCode == 0, $"clang exited with {process.ExitCode}");
            return outputPath;
        }

        [NotNull]
        private static Process Execute(string executable)
        {
            var startInfo = new ProcessStartInfo(executable)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            return Process.Start(startInfo).AssertNotNull();
        }

        private static int ExpectedExitCode([NotNull] string code)
        {
            var exitCode = ExitCodePattern.Match(code).Groups["exitCode"]?.Captures.SingleOrDefault()?.Value ?? "0";
            return int.Parse(exitCode);
        }

        [NotNull]
        private static string ExpectedOutput([NotNull] string code, [NotNull] string channel)
        {
            var regex = new Regex(string.Format(ExpectedOutputFormat, channel).AssertNotNull());
            return regex.Match(code).Groups["output"]?.Captures.SingleOrDefault()?.Value ?? "";
        }

        [NotNull]
        public static TheoryData<TestCase> GetConformanceTestCases()
        {
            var testCases = new TheoryData<TestCase>();
            var testsDirectory = Path.Combine(SolutionDirectory.Get(), "tests");
            foreach (var fullPath in Directory.EnumerateFiles(testsDirectory, "*.ad", SearchOption.AllDirectories).AssertNotNull())
            {
                var relativePath = Path.GetRelativePath(testsDirectory, fullPath);
                testCases.Add(new TestCase(fullPath, relativePath));
            }
            return testCases;
        }
    }
}
