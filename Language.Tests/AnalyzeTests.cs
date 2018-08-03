using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
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
            var codePath = new CodePath(testCase.RelativeCodePath);
            var code = new CodeText(testCase.Code);
            var tokens = new Lexer().Lex(code);
            var parser = new Parser();
            var syntaxTree = parser.Parse(codePath, code, tokens);
            var packageSyntax = new PackageSyntax(syntaxTree.Yield().ToList());
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
                if (actual is IEnumerable<DiagnosticInfo> diagnostics)
                {
                    Assert.Equal(expected.ToObject<int[]>().OrderBy(x => x), diagnostics.Select(d => d.ErrorCode).OrderBy(x => x));
                    return;
                }
                switch (expected.Type)
                {
                    case JTokenType.Boolean:
                        Assert.Equal(expected.ToObject<bool>(), actual);
                        break;
                    case JTokenType.String:
                        switch (actual)
                        {
                            case string actualString:
                                Assert.Equal(expected.ToObject<string>(), actualString);
                                break;
                            case Name _:
                            case DataType _:
                                Assert.Equal(expected.ToObject<string>(), actual.ToString());
                                break;
                            default:
                                Assert.Equal(expected.ToObject(actual.GetType()), actual);
                                break;
                        }
                        break;
                    case JTokenType.Array:
                        var expectedObjects = expected.ToObject<JObject[]>();
                        var actualObjects = (IReadOnlyCollection<object>)actual;
                        Assert.Equal(expectedObjects.Length, actualObjects.Count);
                        foreach (var item in expectedObjects.Zip(actualObjects))
                            AssertSemanticsMatch(item.Item1, item.Item2);
                        break;
                    case JTokenType.Object:
                        AssertSemanticsMatch((JObject)expected, actual);
                        break;
                    case JTokenType.Null:
                        Assert.Null(actual);
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
            var testsDirectory = TestsDirectory.Get();
            var analyzeTestsDirectory = Path.Combine(testsDirectory, "analyze");
            foreach (string testFile in Directory.EnumerateFiles(analyzeTestsDirectory, "*.json", SearchOption.AllDirectories))
            {
                var fullCodePath = Path.ChangeExtension(testFile, "ad");
                var relativeCodePath = Path.GetRelativePath(analyzeTestsDirectory, fullCodePath);
                testCases.Add(new AnalyzeTestCase(fullCodePath, relativeCodePath));
            }
            return testCases;
        }
    }
}
