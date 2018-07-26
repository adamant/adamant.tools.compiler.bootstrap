using System;
using System.IO;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data
{
    /// <summary>
    /// An abstract test case. The visual studio runner serializes tests cases
    /// and doesn't always reload the list of the test cases when running. By
    /// storing the paths to the test files instead of the actual we avoid issues
    /// where the test case has changed but visual studio doesn't pick up the change.
    /// </summary>
    public abstract class TestCase : IXunitSerializable
    {
        public string FullCodePath { get; private set; }
        public string RelativeCodePath { get; private set; }

        private readonly Lazy<string> code;
        public string Code => code.Value;

        [Obsolete("Required by IXunitSerializable", true)]
        public TestCase()
        {
            code = new Lazy<string>(GetCode);
        }

        protected TestCase(string fullCodePath, string relativeCodePath)
        {
            FullCodePath = fullCodePath;
            RelativeCodePath = relativeCodePath;
            code = new Lazy<string>(GetCode);
        }

        protected string GetCode() => File.ReadAllText(FullCodePath, CodeFile.Encoding);

        public override string ToString()
        {
            var pathWithoutExtension = Path.Combine(Path.GetDirectoryName(RelativeCodePath), Path.GetFileNameWithoutExtension(RelativeCodePath));
            return pathWithoutExtension
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
        }

        public virtual void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(FullCodePath), FullCodePath);
            info.AddValue(nameof(RelativeCodePath), RelativeCodePath);
        }
        public virtual void Deserialize(IXunitSerializationInfo info)
        {
            FullCodePath = info.GetValue<string>(nameof(FullCodePath));
            RelativeCodePath = info.GetValue<string>(nameof(RelativeCodePath));
        }
    }
}
