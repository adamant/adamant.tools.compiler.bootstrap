using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    public partial class FunctionBodyParserSpec
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
        private static StatementSyntax ParseStatementWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewFunctionBodyParser();
            var diagnostics = new DiagnosticsBuilder();
            var statementSyntax = parser.ParseStatement(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return statementSyntax;
        }

        [NotNull]
        private static BlockSyntax ParseBlockWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewFunctionBodyParser();
            var diagnostics = new DiagnosticsBuilder();
            var blockSyntax = parser.ParseBlock(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return blockSyntax;
        }
    }
}
