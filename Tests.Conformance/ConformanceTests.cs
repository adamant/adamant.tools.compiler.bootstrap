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
        private readonly ITestOutputHelper testOutput;

        public ConformanceTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public void Can_load_all_conformance_test_cases()
        {
            Assert.NotEmpty(GetConformanceTestCases());
        }

        private static readonly Regex ExitCodePattern = new Regex(@"//[ \t]*exit code: (?<exitCode>\d+)", RegexOptions.Compiled);
        private const string ExpectedOutputFormat = @"\/\*[ \t]*{0}:\r?\n(?<output>(\*+[^/]|[^*])*)\*\/";
        private static readonly Regex ErrorPattern = new Regex(@"//[ \t]*ERROR([ \t].*)?", RegexOptions.Compiled | RegexOptions.Multiline);

        [Theory]
        [MemberData(nameof(GetConformanceTestCases))]
        public void Test_cases(TestCase testCase)
        {
            // Setup
            var codeFile = CodeFile.Load(testCase.FullCodePath);
            var code = codeFile.Code.Text;
            var compiler = new AdamantCompiler();
            var references = new Dictionary<string, Package>();

            // Reference Standard Library
            var stdLibPackage = CompileStdLib(compiler);
            references.Add("adamant.stdlib", stdLibPackage);

            // Analyze
            var package = compiler.CompilePackage("testPackage", codeFile.Yield(), references.ToFixedDictionary());

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

            var errorDiagnostics = diagnostics
                .Where(d => d.Level >= DiagnosticLevel.CompilationError).ToList();

            var assembler = new Assembler();
            testOutput.WriteLine(assembler.Disassemble(package));

            if (expectedCompileErrorLines.Any())
            {
                foreach (var expectedCompileErrorLine in expectedCompileErrorLines)
                {
                    // Assert a single error on the given line
                    var errorsOnLine = errorDiagnostics.Count(e =>
                        e.StartPosition.Line == expectedCompileErrorLine);
                    Assert.True(errorsOnLine == 1,
                        $"Expected single error on line {expectedCompileErrorLine}, found {errorsOnLine}");
                }
            }

            foreach (var error in errorDiagnostics)
            {
                var errorLine = error.StartPosition.Line;
                if (expectedCompileErrorLines.All(line => line != errorLine))
                    Assert.True(false, $"Unexpected error on line {error.StartPosition.Line}");
            }

            // We got only expected errors, but need to not go on to emit code
            if (errorDiagnostics.Any())
                return;

            // Emit Code
            var codePath = Path.ChangeExtension(testCase.FullCodePath, "c");
            EmitCode(package, stdLibPackage, codePath);

            // Compile Code to Executable
            var exePath = CompileToExecutable(codePath);

            // Execute and check results
            var process = Execute(exePath);

            process.WaitForExit();
            var stdout = process.StandardOutput.ReadToEnd();
            testOutput.WriteLine("stdout:");
            testOutput.WriteLine(stdout);
            Assert.Equal(ExpectedOutput(code, "stdout"), stdout);
            var stderr = process.StandardError.ReadToEnd();
            testOutput.WriteLine("stderr:");
            testOutput.WriteLine(stderr);
            Assert.Equal(ExpectedOutput(code, "stderr"), stderr);
            Assert.Equal(ExpectedExitCode(code), process.ExitCode);
        }

        private static Package CompileStdLib(AdamantCompiler compiler)
        {
            var sourceDir = Path.Combine(SolutionDirectory.Get(), @"stdlib\src");
            var sourcePaths = Directory.EnumerateFiles(sourceDir, "*.ad", SearchOption.AllDirectories);
            var rootNamespace = FixedList<string>.Empty;
            var codeFiles = sourcePaths.Select(p => LoadCode(p, sourceDir, rootNamespace)).ToList();
            return compiler.CompilePackage("adamant.stdlib",
                codeFiles,
                FixedDictionary<string, Package>.Empty);
        }

        private static CodeFile LoadCode(
            string path,
            string sourceDir,
            FixedList<string> rootNamespace)
        {
            var relativeDirectory = Path.GetDirectoryName(Path.GetRelativePath(sourceDir, path));
            var ns = rootNamespace.Concat(relativeDirectory.SplitOrEmpty(Path.DirectorySeparatorChar)).ToFixedList();
            return CodeFile.Load(path, ns);
        }

        private static List<int> ExpectedCompileErrorLines(CodeFile codeFile, string code)
        {
            return ErrorPattern.Matches(code)
                .Select(match => codeFile.Code.Lines.LineIndexContainingOffset(match.Index) + 1)
                .ToList();
        }

        private static void EmitCode(Package package, Package stdLibPackage, string path)
        {
            var codeEmitter = new CodeEmitter();
            codeEmitter.Emit(package);
            codeEmitter.Emit(stdLibPackage);
            File.WriteAllText(path, codeEmitter.GetEmittedCode(), Encoding.UTF8);
        }

        private string CompileToExecutable(string codePath)
        {
            var compiler = new CLangCompiler();
            var sourceFiles = new[] { codePath, RuntimeLibraryFixture.GetRuntimeLibraryPath() };
            var headerSearchPaths = new[] { RuntimeLibraryFixture.GetRuntimeDirectory() };
            var outputPath = Path.ChangeExtension(codePath, "exe");
            var exitCode = compiler.Compile(new CompilerOutputAdapter(testOutput), sourceFiles, headerSearchPaths, outputPath);
            Assert.True(exitCode == 0, $"clang exited with {exitCode}");
            return outputPath;
        }

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

            return Process.Start(startInfo);
        }

        private static int ExpectedExitCode(string code)
        {
            var exitCode = ExitCodePattern.Match(code).Groups["exitCode"]?.Captures.SingleOrDefault()?.Value ?? "0";
            return int.Parse(exitCode);
        }

        private static string ExpectedOutput(string code, string channel)
        {
            var regex = new Regex(string.Format(ExpectedOutputFormat, channel));
            return regex.Match(code).Groups["output"]?.Captures.SingleOrDefault()?.Value ?? "";
        }

        public static TheoryData<TestCase> GetConformanceTestCases()
        {
            var testCases = new TheoryData<TestCase>();
            var testsDirectory = Path.Combine(SolutionDirectory.Get(), "tests");
            foreach (var fullPath in Directory.EnumerateFiles(testsDirectory, "*.ad", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(testsDirectory, fullPath);
                testCases.Add(new TestCase(fullPath, relativePath));
            }
            return testCases;
        }
    }
}
