using System;
using System.IO;
using System.Xml.Linq;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class ParseTestCase : IXunitSerializable
    {
        public string CodePath { get; private set; }
        public string Code { get; private set; }
        public ParseTestSyntaxKind SyntaxKind { get; private set; }
        public XElement ExpectedParse { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public ParseTestCase()
        {
        }

        public ParseTestCase(string codePath, string code, ParseTestSyntaxKind syntaxKind, XElement expectedParse)
        {
            CodePath = codePath;
            Code = code;
            SyntaxKind = syntaxKind;
            ExpectedParse = expectedParse;
        }

        public override string ToString()
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(CodePath), Path.GetFileNameWithoutExtension(CodePath));
            var relativePath = Path.GetRelativePath("Parse", pathWithoutExtension);
            return relativePath
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(CodePath), CodePath);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(SyntaxKind), SyntaxKind);
            info.AddValue(nameof(ExpectedParse), ExpectedParse.ToString());
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            CodePath = info.GetValue<string>(nameof(CodePath));
            Code = info.GetValue<string>(nameof(Code));
            SyntaxKind = info.GetValue<ParseTestSyntaxKind>(nameof(SyntaxKind));
            ExpectedParse = XElement.Parse(info.GetValue<string>(nameof(ExpectedParse)));
        }
    }
}
