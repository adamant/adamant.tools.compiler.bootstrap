using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.API;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tests.Conformance.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Conformance
{
    [IntegrationTest]
    [Category("Conformance")]
    [Category("Compile")]
    public class CompileTests
    {
        [NotNull]
        private static readonly Regex ErrorPattern = new Regex(@"// ERROR([ \t].*)?$", RegexOptions.Compiled | RegexOptions.Multiline);

        [Theory]
        [MemberData(nameof(GetAllCompileTestCases))]
        public void Compiles([NotNull] TestCase testCase)
        {
            var codePath = new CodePath(testCase.FullCodePath);
            var codeFile = new CodeFile(codePath, new CodeText(testCase.Code));
            var compiler = new AdamantCompiler();
            var package = compiler.CompilePackage("test.package", codeFile.Yield());

            var expectedErrorLines = ErrorPattern.Matches(testCase.Code)
                .Select(match => codeFile.Code.Lines.LineContainingOffset(match.Index)).ToList();

            var diagnostics = package.Diagnostics;

            if (expectedErrorLines.Count == 0)
                Assert.Empty(diagnostics);
            else
                foreach (var expectedErrorLine in expectedErrorLines)
                {
                    // Assert there is a single diagnostic on the error line
                    Assert.Collection(
                        diagnostics.Where(d => d.StartPosition.Line == expectedErrorLine),
                        d => { }
                    );
                }
        }

        [Fact]
        public void CanGetAllCompileTestCases()
        {
            Assert.NotEmpty(GetAllCompileTestCases());
        }

        /// Loads all *.ad test cases to compile
        public static TheoryData<TestCase> GetAllCompileTestCases()
        {
            var testCases = new TheoryData<TestCase>();
            var testsDirectory = ConformanceTestsDirectory.Get();
            var analyzeTestsDirectory = Path.Combine(testsDirectory, "compile");
            foreach (var codeFile in Directory.EnumerateFiles(analyzeTestsDirectory, "*.ad", SearchOption.AllDirectories))
            {
                var fullCodePath = Path.ChangeExtension(codeFile, "ad");
                var relativeCodePath = Path.GetRelativePath(analyzeTestsDirectory, fullCodePath);
                testCases.Add(new TestCase(fullCodePath, relativeCodePath));
            }
            return testCases;
        }
    }
}
