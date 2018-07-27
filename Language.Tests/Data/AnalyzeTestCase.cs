using System;
using System.IO;
using Adamant.Tools.Compiler.Bootstrap.Framework.Tests.Data;
using Newtonsoft.Json.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class AnalyzeTestCase : TestCase
    {
        private readonly Lazy<JObject> expectedSemanticTree;
        public JObject ExpectedSemanticTree => expectedSemanticTree.Value;

        [Obsolete("Required by IXunitSerializable", true)]
        public AnalyzeTestCase()
        {
            expectedSemanticTree = new Lazy<JObject>(GetExpected);
        }

        public AnalyzeTestCase(string fullCodePath, string relativeCodePath)
            : base(fullCodePath, relativeCodePath)
        {
            expectedSemanticTree = new Lazy<JObject>(GetExpected);
        }

        private JObject GetExpected()
        {
            var testFile = Path.ChangeExtension(FullCodePath, "json");
            var testJson = JObject.Parse(File.ReadAllText(testFile));
            if (testJson.Value<string>("#type") != "test")
                throw new InvalidDataException("Test doesn't have #type: \"test\"");
            return testJson.Value<JObject>("semantic_tree");
        }
    }
}
