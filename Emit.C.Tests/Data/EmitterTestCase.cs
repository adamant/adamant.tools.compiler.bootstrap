using System;
using Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C.Tests.Data
{
    public class EmitterTestCase : TestCase
    {
        public string Code { get; private set; }
        public string Expected { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public EmitterTestCase()
        {
        }

        public EmitterTestCase(string codePath, string code, string expected)
            : base(codePath)
        {
            Code = code;
            Expected = expected;
        }

        public override void Serialize(IXunitSerializationInfo info)
        {
            base.Serialize(info);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(Expected), Expected);
        }

        public override void Deserialize(IXunitSerializationInfo info)
        {
            base.Deserialize(info);
            Code = info.GetValue<string>(nameof(Code));
            Expected = info.GetValue<string>(nameof(Expected));
        }
    }
}
