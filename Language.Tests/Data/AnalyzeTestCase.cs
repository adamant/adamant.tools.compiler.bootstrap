using System;
using Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class AnalyzeTestCase : TestCase
    {
        public string Code { get; private set; }
        public JObject ExpectedSemanticTree { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public AnalyzeTestCase()
        {
        }

        public AnalyzeTestCase(string codePath, string code, JObject expectedSemanticTree)
            : base(codePath)
        {
            Code = code;
            ExpectedSemanticTree = expectedSemanticTree;
        }

        public override void Serialize(IXunitSerializationInfo info)
        {
            base.Serialize(info);
            info.AddValue(nameof(Code), Code);
            info.AddValue(nameof(ExpectedSemanticTree), ExpectedSemanticTree.ToString());
        }

        public override void Deserialize(IXunitSerializationInfo info)
        {
            base.Deserialize(info);
            Code = info.GetValue<string>(nameof(Code));
            ExpectedSemanticTree = JObject.Parse(info.GetValue<string>(nameof(ExpectedSemanticTree)));
        }
    }
}
