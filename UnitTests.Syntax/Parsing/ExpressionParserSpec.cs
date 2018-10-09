using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class ExpressionParserSpec
    {
        [Fact]
        public void Return_void()
        {
            var tokens = FakeTokenStream.From($"return");

            var e = Parse(tokens);

            var r = Assert.IsType<ReturnExpressionSyntax>(e);
            Assert.Equal(tokens[0], r.ReturnKeyword);
            Assert.Null(r.Expression);
        }

        [Theory]
        [InlineData("int")]
        [InlineData("void")]
        [InlineData("bool")]
        [InlineData("uint")]
        [InlineData("never")]
        [InlineData("size")]
        [InlineData("byte")]
        [InlineData("string")]
        public void Primitive_types([NotNull] string text)
        {
            var tokens = FakeTokenStream.FromString(text);

            var e = Parse(tokens);

            var p = Assert.IsType<PrimitiveTypeSyntax>(e);
            Assert.Equal(tokens[0], p.Keyword);
        }

        [NotNull]
        private static ExpressionSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewExpressionParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static ExpressionParser NewExpressionParser()
        {
            var listParser = Fake.ListParser();
            var qualifiedNameParser = Fake.Parser<NameSyntax>();
            return new ExpressionParser(listParser, qualifiedNameParser);
        }
    }
}
