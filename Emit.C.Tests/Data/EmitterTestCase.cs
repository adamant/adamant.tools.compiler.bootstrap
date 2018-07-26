using System;
using System.IO;
using Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C.Tests.Data
{
    public class EmitterTestCase : TestCase
    {
        private readonly Lazy<string> expected;
        public string Expected => expected.Value;

        [Obsolete("Required by IXunitSerializable", true)]
        public EmitterTestCase()
        {
            expected = new Lazy<string>(GetExpected);
        }

        public EmitterTestCase(string fullCodePath, string relativeCodePath)
            : base(fullCodePath, relativeCodePath)
        {
            expected = new Lazy<string>(GetExpected);
        }

        private string GetExpected()
        {
            var testFile = Path.ChangeExtension(FullCodePath, "c");
            return File.ReadAllText(testFile);
        }
    }
}
