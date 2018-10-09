using System;
using System.IO;
using Adamant.Tools.Compiler.Bootstrap.UnitTests;
using Newtonsoft.Json.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.ConformanceTests.Data
{
    public class CompileTestCase : TestCase
    {
        private readonly Lazy<JObject> expectedSemanticTree;
        public JObject ExpectedSemanticTree => expectedSemanticTree.Value;

        [Obsolete("Required by IXunitSerializable", true)]
        public CompileTestCase()
        {
            expectedSemanticTree = new Lazy<JObject>(GetExpected);
        }

        public CompileTestCase(string fullCodePath, string relativeCodePath)
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
