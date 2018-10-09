using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class StatementParserSpec
    {
        [Fact]
        public void Parse_statement_block()
        {
            var statements = Fake.SyntaxList<StatementSyntax>();
            var tokens = FakeTokenStream.From($"{{{statements}}}");

            var b = ParseBlock(tokens);

            Assert.Equal(tokens[0], b.OpenBrace);
            Assert.Equal(statements, b.Statements);
            Assert.Equal(tokens[2], b.CloseBrace);
        }

        [NotNull]
        private static StatementSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewStatementParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static BlockStatementSyntax ParseBlock([NotNull] ITokenStream tokenStream)
        {
            var parser = NewStatementParser();
            return parser.ParseStatementBlock(tokenStream);
        }

        [NotNull]
        private static StatementParser NewStatementParser()
        {
            var listParser = Fake.ListParser();
            var expressionParser = Fake.Parser<ExpressionSyntax>();
            return new StatementParser(listParser, expressionParser);
        }
    }
}
