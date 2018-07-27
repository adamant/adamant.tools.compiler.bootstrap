using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ParseTests
    {
        [Theory]
        [Category("Parse")]
        [MemberData(nameof(GetAllParseTestCases))]
        public void Parses(ParseTestCase testCase)
        {
            var codePath = new CodePath(testCase.RelativeCodePath);
            var code = new CodeText(testCase.Code);
            var tokens = new Lexer().Lex(code);
            var tokenStream = new TokenStream(codePath, code, tokens);
            var parser = new Parser();
            switch (testCase.SyntaxKind)
            {
                case ParseTestSyntaxKind.CompilationUnit:
                    var compilationUnit = parser.ParseCompilationUnit(tokenStream);
                    AssertSyntaxMatches(testCase.ExpectedParse, compilationUnit);
                    break;
                case ParseTestSyntaxKind.Expression:
                    throw new NotImplementedException();
                case ParseTestSyntaxKind.Statment:
                    throw new NotImplementedException();
            }
        }

        [Fact]
        [Category("Parse")]
        public void CanGetAllParseTestCases()
        {
            Assert.NotEmpty(GetAllParseTestCases());
        }

        private void AssertSyntaxMatches(XElement expected, SyntaxNode syntax)
        {
            var expectedKind = expected.Name.LocalName.Replace("_", "");
            Match.On(syntax).With(m => m
                .Is<Token>(t =>
                {
                    Assert.True(t.Kind.ToString().Equals(expectedKind, StringComparison.InvariantCultureIgnoreCase),
                        $"Expected {expectedKind}, found {t.Kind}");
                    var value = expected.Attribute("value");
                    if (value != null)
                        Match.On(t).With(m2 => m2
                            .Is<IdentifierToken>(i => Assert.Equal(value.Value, i.Value)));
                })
                .Is<SyntaxBranchNode>(n =>
                {
                    expectedKind += "Syntax";
                    Assert.True(n.GetType().Name.Equals(expectedKind, StringComparison.InvariantCultureIgnoreCase),
                        $"Expected {expectedKind}, found {n.GetType().Name}");

                    foreach (var attribute in expected.Attributes().Where(a => a.Name != "id"))
                    {
                        var expectedChildIndex = expected.Elements()
                            .Select((e, i) => new { Element = e, Index = i })
                            .Where(x => x.Element.Attribute("id")?.Value == attribute.Value)
                            .Select(x => x.Index)
                            .Single();
                        var expectedChild = n.Children[expectedChildIndex];
                        var actualChild = GetProperty(n, attribute.Name.LocalName.Replace("_", ""));
                        Assert.Equal(expectedChild, actualChild);
                    }

                    foreach (var child in expected.Elements().Zip(n.Children))
                        AssertSyntaxMatches(child.Item1, child.Item2);
                })
            );
        }

        private SyntaxNode GetProperty(SyntaxNode node, string name)
        {
            var property = node.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            Assert.True(property != null, $"No property '{name}' on node type {node.GetType().Name}");
            return (SyntaxNode)property.GetValue(node);
        }

        /// Loads all *.xml test cases for parsing.
        public static TheoryData<ParseTestCase> GetAllParseTestCases()
        {
            var testCases = new TheoryData<ParseTestCase>();
            var testsDirectory = TestsDirectory.Get();
            var parseTestsDirectory = Path.Combine(testsDirectory, "parse");
            foreach (string testFile in Directory.EnumerateFiles(parseTestsDirectory, "*.xml", SearchOption.AllDirectories))
            {
                var fullCodePath = Path.ChangeExtension(testFile, "ad");
                var relativeCodePath = Path.GetRelativePath(parseTestsDirectory, fullCodePath);
                testCases.Add(new ParseTestCase(fullCodePath, relativeCodePath));
            }
            return testCases;
        }
    }
}