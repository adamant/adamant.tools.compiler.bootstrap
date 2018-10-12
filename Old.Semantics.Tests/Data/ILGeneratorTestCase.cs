using System;
using System.IO;
using Adamant.Tools.Compiler.Bootstrap.UnitTests;
using Adamant.Tools.Compiler.Bootstrap.UnitTests.Helpers;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Tests.Data
{
    public class ILGeneratorTestCase : TestCase
    {
        public string ExpectedILPath { get; private set; }
        private readonly Lazy<string> expectedIL;
        public string ExpectedIL => expectedIL.Value;

        [Obsolete("Required by IXunitSerializable", true)]
        public ILGeneratorTestCase()
        {
            expectedIL = new Lazy<string>(GetExpected);
        }

        public ILGeneratorTestCase(string fullCodePath, string relativeCodePath, string expectedILPath)
            : base(fullCodePath, relativeCodePath)
        {
            ExpectedILPath = expectedILPath;
            expectedIL = new Lazy<string>(GetExpected);
        }

        public override void Serialize(IXunitSerializationInfo info)
        {
            base.Serialize(info);
            info.AddValue(nameof(ExpectedILPath), ExpectedILPath);
        }
        public override void Deserialize(IXunitSerializationInfo info)
        {
            base.Deserialize(info);
            ExpectedILPath = info.GetValue<string>(nameof(ExpectedILPath));
        }

        private string GetExpected()
        {
            return File.ReadAllText(ExpectedILPath);
        }
    }
}
