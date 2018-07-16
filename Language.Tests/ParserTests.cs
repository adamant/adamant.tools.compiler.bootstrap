using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetAllParseTestCases))]
        public void Parses(ParseTestCase testCase)
        {
            // TODO
        }

        /// Loads all *.xml test cases for parsing.
        public static TheoryData<ParseTestCase> GetAllParseTestCases()
        {
            var testCases = new TheoryData<ParseTestCase>();
            var parseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Parse");
            foreach (string testFile in Directory.EnumerateFiles(parseDirectory, "*.xml", SearchOption.AllDirectories))
            {
                var codeFile = Path.ChangeExtension(testFile, "ad");
                var code = File.ReadAllText(codeFile, CodeFile.Encoding);
                var testXml = XDocument.Load(testFile).Element("test");
                var codeXml = testXml.Element("code");
                var syntaxKind = Enum.Parse<ParseTestSyntaxKind>(codeXml.Attribute("kind").Value.Replace("_", ""), true);
                var rootSyntaxXml = testXml.Element("syntax").Ancestors().Single();
                testCases.Add(new ParseTestCase(code, syntaxKind, rootSyntaxXml));
            }
            return testCases;
        }
    }
}
