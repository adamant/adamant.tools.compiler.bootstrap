using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
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
    public class DeclarationParserSpec
    {
        [Fact]
        public void Function_declaration()
        {
            var accessModifer = FakeSyntax.AccessModifier();
            var returnExpression = FakeSyntax.Expression();
            var parameters = FakeSyntax.SeparatedList<ParameterSyntax>();
            var blockStatement = FakeSyntax.BlockStatement();
            var tokens = FakeTokenStream.From($"{accessModifer} fn function_name({parameters}) -> {returnExpression} {blockStatement}");

            var d = ParseWithoutError(tokens);

            var f = Assert.IsType<FunctionDeclarationSyntax>(d);
            Assert.Equal(accessModifer, f.AccessModifier);
            Assert.Equal(tokens[1], f.FunctionKeyword);
            Assert.Equal(tokens[2], f.Name);
            Assert.Equal(tokens[3], f.OpenParen);
            Assert.Equal(parameters, f.Parameters);
            Assert.Equal(tokens[5], f.CloseParen);
            Assert.Equal(tokens[6], f.Arrow);
            Assert.Equal(returnExpression, f.ReturnTypeExpression);
            Assert.Equal(blockStatement, f.Body);
        }

        [Fact]
        public void Class_declaration()
        {
            var accessModifer = FakeSyntax.AccessModifier();
            var tokens = FakeTokenStream.From($"{accessModifer} class Class_Name {{}}");

            var d = ParseWithoutError(tokens);

            var c = Assert.IsType<ClassDeclarationSyntax>(d);
            Assert.Equal(accessModifer, c.AccessModifier);
            Assert.Equal(tokens[1], c.ClassKeyword);
            Assert.Equal(tokens[2], c.Name);
            Assert.Equal(tokens[3], c.OpenBrace);
            Assert.Equal(tokens[4], c.CloseBrace);
        }

        [NotNull]
        private static DeclarationSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewDeclarationParser();
            var diagnostics = new DiagnosticsBuilder();
            var declarationSyntax = parser.Parse(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return declarationSyntax;
        }

        [NotNull]
        private static DeclarationParser NewDeclarationParser()
        {
            var listParser = FakeParser.ForLists();
            var expressionParser = FakeParser.For<ExpressionSyntax>();
            var blockStatementParser = FakeParser.For<BlockStatementSyntax>();
            var parameterParser = FakeParser.For<ParameterSyntax>();
            var accessModifierParser = FakeParser.For<AccessModifierSyntax>();
            return new DeclarationParser(
                listParser,
                expressionParser,
                blockStatementParser,
                parameterParser,
                accessModifierParser);
        }
    }
}
