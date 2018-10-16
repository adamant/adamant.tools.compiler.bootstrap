using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class StatementParserSpec
    {
        [Fact]
        public void Parse_statement_block()
        {
            var statements = FakeSyntax.List<StatementSyntax>();
            var tokens = FakeTokenStream.From($"{{{statements}}}");

            var b = ParseBlockWithoutError(tokens);

            Assert.Equal(tokens[0], b.OpenBrace);
            Assert.Equal(statements, b.Statements);
            Assert.Equal(tokens[2], b.CloseBrace);
        }

        [NotNull]
        private static StatementSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewStatementParser();
            var diagnostics = new DiagnosticsBuilder();
            var statementSyntax = parser.Parse(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return statementSyntax;
        }

        [NotNull]
        private static BlockStatementSyntax ParseBlockWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewStatementParser();
            var diagnostics = new DiagnosticsBuilder();
            var blockStatementSyntax = parser.ParseBlockStatement(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return blockStatementSyntax;
        }

        [NotNull]
        private static StatementParser NewStatementParser()
        {
            var listParser = FakeParser.ForLists();
            var expressionParser = FakeParser.For<ExpressionSyntax>();
            return new StatementParser(listParser, expressionParser);
        }
    }
}
