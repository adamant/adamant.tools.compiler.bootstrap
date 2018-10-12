using System.IO;
using Adamant.Tools.Compiler.Bootstrap.ConformanceTests.Data;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.UnitTests;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Tests
{
    public class ILGeneratorTests
    {
        [Theory]
        [Category("Analyze")]
        [MemberData(nameof(GetAllILGeneratorTestCases))]
        public void Analyzes(ILGeneratorTestCase testCase)
        {
            var codePath = new CodePath(testCase.RelativeCodePath);
            var code = new CodeText(testCase.Code);
            var file = new CodeFile(codePath, code);
            var tokens = new Lexer().Lex(file);
            var parser = new Parser();
            var compilationUnit = parser.Parse(file, tokens);
            var packageSyntax = new PackageSyntax("test.package", compilationUnit.ToSyntaxList());
            var analyzer = new SemanticAnalyzer();
            var il = analyzer.Analyze(packageSyntax).IL;
            Assert.Equal(testCase.ExpectedIL, il.ToString());
        }

        [Fact]
        [Category("Analyze")]
        public void CanGetAllILGeneratorTestCases()
        {
            Assert.NotEmpty(GetAllILGeneratorTestCases());
        }

        /// Loads all *.ail test cases for the lang tests.
        public static TheoryData<ILGeneratorTestCase> GetAllILGeneratorTestCases()
        {
            var testCases = new TheoryData<ILGeneratorTestCase>();
            var projectDirectory = ProjectDirectory.Get();
            var testsDirectory = Path.Combine(projectDirectory, "IL", "langTests");
            var langTestsDirectory = LangTestsDirectory.Get();
            foreach (var expectedILFile in Directory.EnumerateFiles(testsDirectory, "*.ail", SearchOption.AllDirectories))
            {
                var relativeCodePath = Path.GetRelativePath(testsDirectory, Path.ChangeExtension(expectedILFile, "ad"));
                var fullCodePath = Path.Combine(langTestsDirectory, relativeCodePath);

                testCases.Add(new ILGeneratorTestCase(fullCodePath, relativeCodePath, expectedILFile));
            }
            return testCases;
        }
    }
}
