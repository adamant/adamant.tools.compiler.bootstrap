using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class AnalyzeTests
    {
        [Theory]
        [Category("Analyze")]
        [MemberData(nameof(GetAllAnalyzerTestCases))]
        public void Analyzes(AnalyzeTestCase testCase)
        {
            var codePath = new CodePath(testCase.CodePath);
            var code = new CodeText(testCase.Code);
            var tokens = new Lexer().Lex(code);
            var parser = new Parser();
            var syntaxTree = parser.Parse(codePath, code, tokens);
            var packageSyntax = new PackageSyntax(syntaxTree.Yield());
            var analyzer = new SemanticAnalyzer();
            var package = analyzer.Analyze(packageSyntax);
            AssertSemanticsMatch(testCase.ExpectedSemanticTree, package);
        }

        [Fact]
        [Category("Analyze")]
        public void CanGetAllAnalyzerTestCases()
        {
            Assert.NotEmpty(GetAllAnalyzerTestCases());
        }

        private void AssertSemanticsMatch(JObject expectedValue, object value)
        {
            // TODO  Finish checking semantics matches expected
            var expectedType = expectedValue.Value<string>("#type").Replace("_", "");
            Assert.True(value.GetType().Name.Equals(expectedType, StringComparison.InvariantCultureIgnoreCase),
                        $"Expected {expectedType}, found {value.GetType().Name}");

            foreach (var property in expectedValue.Properties().Where(p => p.Name != "#type"))
            {
                var expected = property.Value;
                var actual = GetProperty(value, property.Name.Replace("_", ""));
                switch (expected.Type)
                {
                    case JTokenType.Boolean:
                        Assert.Equal(expected.ToObject<bool>(), actual);
                        break;
                    case JTokenType.String:
                        if (actual is string actualString)
                            Assert.Equal(expected.ToObject<string>(), actualString);
                        else
                            Assert.Equal(expected.ToObject(actual.GetType()), actual);
                        break;
                    case JTokenType.Array:
                        var expectedObjects = expected.ToObject<JObject[]>();
                        var actualObjects = (IEnumerable<object>)actual;
                        Assert.Equal(expectedObjects.Length, actualObjects.Count());
                        foreach (var item in expectedObjects.Zip(actualObjects))
                            AssertSemanticsMatch(item.Item1, item.Item2);
                        break;
                    case JTokenType.Object:
                        AssertSemanticsMatch((JObject)expected, actual);
                        break;
                    default:
                        throw new NotSupportedException(expected.Type.ToString());
                }
            }
        }

        private object GetProperty(object value, string name)
        {
            var property = value.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            Assert.True(property != null, $"No property '{name}' on type {value.GetType().Name}");
            return property.GetValue(value);
        }

        /// Loads all *.xml test cases for the analyzer.
        public static TheoryData<AnalyzeTestCase> GetAllAnalyzerTestCases()
        {
            var testCases = new TheoryData<AnalyzeTestCase>();
            var currentDirectory = Directory.GetCurrentDirectory();
            var analyzeTestsDirectory = Path.Combine(currentDirectory, "Analyze");
            foreach (string testFile in Directory.EnumerateFiles(analyzeTestsDirectory, "*.json", SearchOption.AllDirectories))
            {
                var codeFile = Path.ChangeExtension(testFile, "ad");
                var codePath = Path.GetRelativePath(analyzeTestsDirectory, codeFile);
                var code = File.ReadAllText(codeFile, CodeFile.Encoding);
                var testJson = JObject.Parse(File.ReadAllText(testFile));
                if (testJson.Value<string>("#type") != "test")
                    throw new InvalidDataException("Test doesn't have #type: \"test\"");
                var expectedSemanticTreeJson = testJson.Value<JObject>("semantic_tree");
                testCases.Add(new AnalyzeTestCase(codePath, code, expectedSemanticTreeJson));
            }
            return testCases;
        }
    }
}
