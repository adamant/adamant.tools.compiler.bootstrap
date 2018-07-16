using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetAllParseTestCases))]
        public void Parses(ParseTestCase testCase)
        {
            var code = new CodeText(testCase.Code);
            var tokens = new Lexer().Lex(code);
            var tokenStream = new TokenStream(testCase.CodePath, code, tokens);
            var parser = new Parser();
            switch (testCase.SyntaxKind)
            {
                case ParseTestSyntaxKind.CompilationUnit:
                    //var compilationUnit = parser.ParseCompilationUnit(tokenStream);
                    // TODO validate syntax tree
                    break;
                case ParseTestSyntaxKind.Expression:
                    throw new NotImplementedException();
                case ParseTestSyntaxKind.Statment:
                    throw new NotImplementedException();
            }
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
                var codePath = new CodePath(Path.GetRelativePath(currentDirectory, codeFile));
                var code = File.ReadAllText(codeFile, CodeFile.Encoding);
                var testXml = XDocument.Load(testFile).Element("test");
                var codeXml = testXml.Element("code");
                var syntaxKind = Enum.Parse<ParseTestSyntaxKind>(codeXml.Attribute("kind").Value.Replace("_", ""), true);
                var rootSyntaxXml = testXml.Element("syntax").Ancestors().Single();
                testCases.Add(new ParseTestCase(codePath, code, syntaxKind, rootSyntaxXml));
            }
            return testCases;
        }
    }
}
