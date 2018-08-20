using System.IO;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Tests
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
            var tokens = new Lexer().Lex(code);
            var parser = new Parser();
            var syntaxTree = parser.Parse(codePath, code, tokens);
            var packageSyntax = new PackageSyntax(syntaxTree.Yield().ToList());
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
