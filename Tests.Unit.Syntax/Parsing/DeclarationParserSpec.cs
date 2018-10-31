using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types;
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
            var genericParameters = FakeSyntax.GenericParameters();
            var parameters = FakeSyntax.SeparatedList<ParameterSyntax>();
            var block = FakeSyntax.Block();
            var tokens = FakeTokenStream.From($"{accessModifer} fn function_name{genericParameters}({parameters}) -> {returnExpression} {block}");

            var d = ParseWithoutError(tokens);

            var f = Assert.IsType<NamedFunctionDeclarationSyntax>(d);
            Assert.Single(f.Modifiers, accessModifer);
            Assert.Equal(tokens[1], f.FunctionKeyword);
            Assert.Equal(tokens[2], f.Name);
            Assert.Equal(genericParameters, f.GenericParameters);
            Assert.Equal(tokens[4], f.OpenParen);
            Assert.Equal(parameters, f.ParameterList);
            Assert.Equal(tokens[6], f.CloseParen);
            Assert.Equal(tokens[7], f.Arrow);
            Assert.Equal(returnExpression, f.ReturnTypeExpression);
            Assert.Equal(block, f.Body);
        }

        [Fact]
        public void Class_declaration()
        {
            var accessModifer = FakeSyntax.AccessModifier();
            var genericParameters = FakeSyntax.GenericParameters();
            var members = FakeSyntax.List<MemberDeclarationSyntax>();
            var tokens = FakeTokenStream.From($"{accessModifer} class Class_Name{genericParameters} {{{members}}}");

            var d = ParseWithoutError(tokens);

            var c = Assert.IsType<ClassDeclarationSyntax>(d);
            Assert.Single(c.Modifiers, accessModifer);
            Assert.Equal(tokens[1], c.ClassKeyword);
            Assert.Equal(tokens[2], c.Name);
            Assert.Equal(genericParameters, c.GenericParameters);
            Assert.Equal(tokens[4], c.OpenBrace);
            Assert.Equal(members, c.Members);
            Assert.Equal(tokens[6], c.CloseBrace);
        }

        [NotNull]
        private static DeclarationSyntax ParseWithoutError([NotNull] ITokenStream tokenStream)
        {
            var parser = NewDeclarationParser();
            var diagnostics = new DiagnosticsBuilder();
            var declarationSyntax = parser.ParseDeclaration(tokenStream, diagnostics);
            Assert.Empty(diagnostics.Build());
            return declarationSyntax;
        }

        [NotNull]
        private static DeclarationParser NewDeclarationParser()
        {
            var listParser = FakeParser.ForLists();
            var expressionParser = FakeParser.ForExpressions();
            var blockParser = FakeParser.ForBlocks();
            var parameterParser = FakeParser.ForParameters();
            var modifierParser = FakeParser.ForModifiers();
            var genericsParser = FakeParser.ForGenerics();
            var nameParser = FakeParser.ForNames();
            return new DeclarationParser(
                listParser,
                expressionParser,
                blockParser,
                parameterParser,
                modifierParser,
                genericsParser,
                nameParser);
        }
    }
}
