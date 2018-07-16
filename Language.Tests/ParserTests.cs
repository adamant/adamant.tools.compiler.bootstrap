using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tree;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ParserTests
    {
        [Theory]
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
            Match.On(syntax).With(m => m
                .Is<Token>(t =>
                {
                    var expectedKind = expected.Name.LocalName.Replace("_", "");
                })
                .Is<SyntaxBranchNode>(n => { })
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
                var expectedParseXml = testXml.Element("expected_parse").Ancestors().Single();
                testCases.Add(new ParseTestCase(codePath, code, syntaxKind, expectedParseXml));
            }
            return testCases;
        }
    }
}
