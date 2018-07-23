using System;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class ParseTestCase : TestCase
    {
        public string Code { get; private set; }
        public ParseTestSyntaxKind SyntaxKind { get; private set; }
        public XElement ExpectedParse { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public ParseTestCase()
        {
        }

        public ParseTestCase(string codePath, string code, ParseTestSyntaxKind syntaxKind, XElement expectedParse)
            : base(codePath)
        {
            Code = code;
            SyntaxKind = syntaxKind;
            ExpectedParse = expectedParse;
        }

        public override void Serialize(IXunitSerializationInfo info)
        {
            base.Serialize(info);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(SyntaxKind), SyntaxKind);
            info.AddValue(nameof(ExpectedParse), ExpectedParse.ToString());
        }

        public override void Deserialize(IXunitSerializationInfo info)
        {
            base.Deserialize(info);
            Code = info.GetValue<string>(nameof(Code));
            SyntaxKind = info.GetValue<ParseTestSyntaxKind>(nameof(SyntaxKind));
            ExpectedParse = XElement.Parse(info.GetValue<string>(nameof(ExpectedParse)));
        }
    }
}
