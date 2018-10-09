using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
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
    public class DeclarationParserSpec
    {
        [Fact]
        public void Function_declaration()
        {
            var accessModifer = Fake.AccessModifier();
            var returnExpression = Fake.Expression();
            var parameters = Fake.SeparatedList<ParameterSyntax>();
            var blockStatement = Fake.BlockStatement();
            var tokens = FakeTokenStream.From($"{accessModifer} fn function_name({parameters}) -> {returnExpression} {blockStatement}");

            var e = Parse(tokens);

            var f = Assert.IsType<FunctionDeclarationSyntax>(e);
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

        [NotNull]
        private static DeclarationSyntax Parse([NotNull] ITokenStream tokenStream)
        {
            var parser = NewDeclarationParser();
            return parser.Parse(tokenStream);
        }

        [NotNull]
        private static DeclarationParser NewDeclarationParser()
        {
            var listParser = Fake.ListParser();
            var expressionParser = Fake.Parser<ExpressionSyntax>();
            var blockStatementParser = Fake.Parser<BlockStatementSyntax>();
            var parameterParser = Fake.Parser<ParameterSyntax>();
            var accessModifierParser = Fake.Parser<AccessModifierSyntax>();
            return new DeclarationParser(
                listParser,
                expressionParser,
                blockStatementParser,
                parameterParser,
                accessModifierParser);
        }
    }
}
