using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    public partial class FunctionBodyParserSpec
    {
        [Fact]
        public void Return_void()
        {
            var tokens = FakeTokenStream.From($"return");

            var e = ParseExpressionWithoutError(tokens);

            var r = Assert.IsType<ReturnExpressionSyntax>(e);
            Assert.Equal(tokens[0], r.ReturnKeyword);
            Assert.Null(r.ReturnValue);
        }

        [Fact]
        public void Owned_lifetime_type()
        {
            var tokens = FakeTokenStream.From($"Test$owned");

            var e = ParseExpressionWithoutError(tokens);

            var t = Assert.IsType<LifetimeTypeSyntax>(e);
            var identifierName = Assert.IsType<IdentifierNameSyntax>(t.TypeExpression);
            Assert.Equal(tokens[0], identifierName.Name);
            Assert.Equal(tokens[1], t.Operator);
            Assert.Equal(tokens[2], t.Lifetime);
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

            var e = ParseExpressionWithoutError(tokens);

            var p = Assert.IsType<PrimitiveTypeSyntax>(e);
            Assert.Equal(tokens[0], p.Keyword);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("+")]
        [InlineData("@")]
        [InlineData("^")]
        [InlineData("not")]
        public void Unary_prefix_operators([NotNull] string text)
        {
            var tokens = FakeTokenStream.FromString(text + " x");

            var e = ParseExpressionWithoutError(tokens);

            var op = Assert.IsType<UnaryOperatorExpressionSyntax>(e);
            Assert.Equal(tokens[0], op.Operator);
            var operand = Assert.IsType<IdentifierNameSyntax>(op.Operand);
            Assert.Equal(tokens[1], operand.Name);
        }

        [Theory]
        [InlineData("and")]
        [InlineData("or")]
        [InlineData("xor")]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        [InlineData("=")]
        [InlineData("+=")]
        [InlineData("-=")]
        [InlineData("*=")]
        [InlineData("/=")]
        [InlineData("==")]
        [InlineData("=/=")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData(">=")]
        [InlineData("..")]
        [InlineData(".")]
        public void Binary_operators([NotNull] string text)
        {
            var tokens = FakeTokenStream.FromString("x " + text + " y");

            var e = ParseExpressionWithoutError(tokens);

            var op = Assert.IsType<BinaryOperatorExpressionSyntax>(e);
            var leftOperand = Assert.IsType<IdentifierNameSyntax>(op.LeftOperand);
            Assert.Equal(tokens[0], leftOperand.Name);
            Assert.Equal(tokens[1], op.Operator);
            var rightOperand = Assert.IsType<IdentifierNameSyntax>(op.RightOperand);
            Assert.Equal(tokens[2], rightOperand.Name);
        }

        [NotNull]
        private static ExpressionSyntax ParseExpressionWithoutError([NotNull] ITokenStream tokens)
        {
            var parser = NewFunctionBodyParser();
            var diagnostics = new DiagnosticsBuilder();
            var expressionSyntax = parser.ParseExpression(tokens, diagnostics);
            Assert.Empty(diagnostics.Build());
            return expressionSyntax;
        }

        [NotNull]
        private static FunctionBodyParser NewFunctionBodyParser()
        {
            var listParser = FakeParser.ForLists();
            return new FunctionBodyParser(listParser);
        }
    }
}
