using System;
using System.IO;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C.Tests.Data
{
    public class EmitterTestCase : IXunitSerializable
    {
        public string CodePath { get; private set; }
        public string Code { get; private set; }
        public string Expected { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public EmitterTestCase()
        {
        }

        public EmitterTestCase(string codePath, string code, string expected)
        {
            CodePath = codePath;
            Code = code;
            Expected = expected;
        }

        public override string ToString()
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(CodePath), Path.GetFileNameWithoutExtension(CodePath));
            return pathWithoutExtension
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(CodePath), CodePath);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(Expected), Expected);
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            CodePath = info.GetValue<string>(nameof(CodePath));
            Code = info.GetValue<string>(nameof(Code));
            Expected = info.GetValue<string>(nameof(Expected));
        }
    }
}
