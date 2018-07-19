using System;
using System.IO;
using System.Xml.Linq;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class AnalyzerTestCase : IXunitSerializable
    {
        public string CodePath { get; private set; }
        public string Code { get; private set; }
        public XElement ExpectedSemanticTree { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public AnalyzerTestCase()
        {
        }

        public AnalyzerTestCase(string codePath, string code, XElement expectedSemanticTree)
        {
            CodePath = codePath;
            Code = code;
            ExpectedSemanticTree = expectedSemanticTree;
        }

        public override string ToString()
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(CodePath), Path.GetFileNameWithoutExtension(CodePath));
            var relativePath = Path.GetRelativePath("Analyze", pathWithoutExtension);
            return relativePath
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(CodePath), CodePath);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(ExpectedSemanticTree), ExpectedSemanticTree.ToString());
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            CodePath = info.GetValue<string>(nameof(CodePath));
            Code = info.GetValue<string>(nameof(Code));
            ExpectedSemanticTree = XElement.Parse(info.GetValue<string>(nameof(ExpectedSemanticTree)));
        }
    }
}
