using System;
using Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class RunTestCase : TestCase
    {
        public string Code { get; private set; }
        public string Stdout { get; private set; }
        public string Stderr { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public RunTestCase()
        {
        }

        public RunTestCase(string codePath, string code, string stdout, string stderr)
            : base(codePath)
        {
            Code = code;
            Stdout = stdout;
            Stderr = stderr;
        }

        public override void Serialize(IXunitSerializationInfo info)
        {
            base.Serialize(info);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(Stdout), Stdout);
            info.AddValue(nameof(Stderr), Stderr);
        }

        public override void Deserialize(IXunitSerializationInfo info)
        {
            base.Deserialize(info);
            Code = info.GetValue<string>(nameof(Code));
            Stdout = info.GetValue<string>(nameof(Stdout));
            Stderr = info.GetValue<string>(nameof(Stderr));
        }
    }
}
