using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Parsing
{
    [UnitTest]
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

        [NotNull]
        private static ExpressionSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewExpressionParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static ExpressionParser NewExpressionParser()
        {
            var listParser = new FakeListParser();
            var qualifiedNameParser = new FakeQualifiedNameParser();
            return new ExpressionParser(listParser, qualifiedNameParser);
        }
    }
}
