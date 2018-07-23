using System;
using System.IO;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data
{
    public abstract class TestCase : IXunitSerializable
    {
        public string CodePath { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public TestCase()
        {
        }

        protected TestCase(string codePath)
        {
            CodePath = codePath;
        }

        public override string ToString()
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(CodePath), Path.GetFileNameWithoutExtension(CodePath));
            return pathWithoutExtension
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
        }

        public virtual void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(CodePath), CodePath);
        }
        public virtual void Deserialize(IXunitSerializationInfo info)
        {
            CodePath = info.GetValue<string>(nameof(CodePath));
        }
    }
}
