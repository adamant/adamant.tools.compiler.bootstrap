using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ParserTests
    {
        [Theory]
        [Category("Parser")]
        [MemberData(nameof(GetAllParseTestCases))]
        public void Parses(ParseTestCase testCase)
        {
            var codePath = new CodePath(testCase.CodePath);
            var code = new CodeText(testCase.Code);
            var tokens = new Lexer().Lex(code);
            var tokenStream = new TokenStream(codePath, code, tokens);
            var parser = new Parser();
            switch (testCase.SyntaxKind)
            {
                case ParseTestSyntaxKind.CompilationUnit:
                    var compilationUnit = parser.ParseCompilationUnit(tokenStream);
                    AssertSyntaxMatches(testCase.ExpectedParse, compilationUnit);
                    break;
                case ParseTestSyntaxKind.Expression:
                    throw new NotImplementedException();
                case ParseTestSyntaxKind.Statment:
                    throw new NotImplementedException();
            }
        }

        private void AssertSyntaxMatches(XElement expected, SyntaxNode syntax)
        {
            // TODO  Finish checking syntax matches expected
            var expectedKind = expected.Name.LocalName.Replace("_", "");
            Match.On(syntax).With(m => m
                .Is<Token>(t =>
                {
                    Assert.True(t.Kind.ToString().Equals(expectedKind, StringComparison.InvariantCultureIgnoreCase),
                        $"Expected {expectedKind}, found {t.Kind}");
                    // TODO check value
                })
                .Is<SyntaxBranchNode>(n =>
                {
                    expectedKind += "Syntax";
                    Assert.True(n.GetType().Name.Equals(expectedKind, StringComparison.InvariantCultureIgnoreCase),
                        $"Expected {expectedKind}, found {n.GetType().Name}");
                    // TODO Check Attributes

                    foreach (var child in expected.Elements().Zip(n.Children))
                        AssertSyntaxMatches(child.Item1, child.Item2);
                })
            );
        }

        /// Loads all *.xml test cases for parsing.
        public static TheoryData<ParseTestCase> GetAllParseTestCases()
        {
            var testCases = new TheoryData<ParseTestCase>();
            var currentDirectory = Directory.GetCurrentDirectory();
            var parseDirectory = Path.Combine(currentDirectory, "Parse");
            foreach (string testFile in Directory.EnumerateFiles(parseDirectory, "*.xml", SearchOption.AllDirectories))
            {
                var codeFile = Path.ChangeExtension(testFile, "ad");
                var codePath = Path.GetRelativePath(currentDirectory, codeFile);
                var code = File.ReadAllText(codeFile, CodeFile.Encoding);
                var testXml = XDocument.Load(testFile).Element("test");
                var codeXml = testXml.Element("code");
                var syntaxKind = Enum.Parse<ParseTestSyntaxKind>(codeXml.Attribute("kind").Value.Replace("_", ""), true);
                var expectedParseXml = testXml.Element("expected_parse").Elements().Single();
                testCases.Add(new ParseTestCase(codePath, code, syntaxKind, expectedParseXml));
            }
            return testCases;
        }
    }
}
