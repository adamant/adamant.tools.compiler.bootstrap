using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class SemanticAnalyzerTests
    {
        [Theory]
        [Category("Semantic Analyzer")]
        [MemberData(nameof(GetAllAnalyzerTestCases))]
        public void Analyzes(AnalyzerTestCase testCase)
        {
            var codePath = new CodePath(testCase.CodePath);
            var code = new CodeText(testCase.Code);
            var tokens = new Lexer().Lex(code);
            var parser = new Parser();
            var syntaxTree = parser.Parse(codePath, code, tokens);
            var packageSyntax = new PackageSyntax(syntaxTree.Yield());
            var analyzer = new SemanticAnalyzer();
            var package = analyzer.Analyze(packageSyntax);
            AssertSemanticsMatch(testCase.ExpectedSemanticTree, package);
        }

        private void AssertSemanticsMatch(XElement expected, SemanticNode node)
        {
            // TODO  Finish checking semantics matches expected
            var expectedKind = expected.Name.LocalName.Replace("_", "");
            Assert.True(node.GetType().Name.Equals(expectedKind, StringComparison.InvariantCultureIgnoreCase),
                        $"Expected {expectedKind}, found {node.GetType().Name}");
            // TODO Check Attributes

            foreach (var child in expected.Elements().Zip(node.Children))
                AssertSemanticsMatch(child.Item1, child.Item2);
        }

        /// Loads all *.xml test cases for the analyzer.
        public static TheoryData<AnalyzerTestCase> GetAllAnalyzerTestCases()
        {
            var testCases = new TheoryData<AnalyzerTestCase>();
            var currentDirectory = Directory.GetCurrentDirectory();
            var parseDirectory = Path.Combine(currentDirectory, "Analyze");
            foreach (string testFile in Directory.EnumerateFiles(parseDirectory, "*.xml", SearchOption.AllDirectories))
            {
                var codeFile = Path.ChangeExtension(testFile, "ad");
                var codePath = Path.GetRelativePath(currentDirectory, codeFile);
                var code = File.ReadAllText(codeFile, CodeFile.Encoding);
                var testXml = XDocument.Load(testFile).Element("test");
                var expectedSemanticTreeXml = testXml.Element("semantic_tree").Elements().Single();
                testCases.Add(new AnalyzerTestCase(codePath, code, expectedSemanticTreeXml));
            }
            return testCases;
        }
    }
}
