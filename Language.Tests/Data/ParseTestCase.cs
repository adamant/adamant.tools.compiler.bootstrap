using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.UnitTests.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class ParseTestCase : TestCase
    {
        private readonly Lazy<XElement> testXml;
        private readonly Lazy<ParseTestSyntaxKind> syntaxKind;
        public ParseTestSyntaxKind SyntaxKind => syntaxKind.Value;
        private readonly Lazy<XElement> expectedParse;
        public XElement ExpectedParse => expectedParse.Value;

        [Obsolete("Required by IXunitSerializable", true)]
        public ParseTestCase()
        {
            testXml = new Lazy<XElement>(GetTestXml);
            syntaxKind = new Lazy<ParseTestSyntaxKind>(GetSyntaxKind);
            expectedParse = new Lazy<XElement>(GetExpectedParse);
        }

        public ParseTestCase(string fullCodePath, string relativeCodePath)
            : base(fullCodePath, relativeCodePath)
        {
            testXml = new Lazy<XElement>(GetTestXml);
            syntaxKind = new Lazy<ParseTestSyntaxKind>(GetSyntaxKind);
            expectedParse = new Lazy<XElement>(GetExpectedParse);
        }

        private XElement GetTestXml()
        {
            var testFile = Path.ChangeExtension(FullCodePath, "xml");
            return XDocument.Load(testFile).Element("test");
        }

        private ParseTestSyntaxKind GetSyntaxKind()
        {
            var codeXml = testXml.Value.Element("code");
            return Enum.Parse<ParseTestSyntaxKind>(codeXml.Attribute("kind").Value.Replace("_", ""), true);
        }

        private XElement GetExpectedParse()
        {
            return testXml.Value.Element("expected_parse").Elements().Single();
        }
    }
}
