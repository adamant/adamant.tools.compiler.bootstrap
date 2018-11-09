using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Helpers;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class ListParserSpec
    {
        [Fact]
        public void Empty_list()
        {
            var tokens = FakeTokenStream.From($"");
            var diagnostics = new DiagnosticsBuilder();

            var l = new ListParser().ParseList(tokens, NotCalled, TypeOf<CloseParenToken>(), diagnostics);

            Assert.Empty(diagnostics.Build());
            Assert.Empty(l);
        }

        [Fact]
        public void One_item_list()
        {
            var item1 = FakeSyntax.Expression();
            var tokens = FakeTokenStream.From($"{item1}");

            var l = ParseListWithoutError<CloseParenToken>(tokens);

            Assert.Collection(l, i => Assert.Equal(item1, i));
        }

        [Fact]
        public void Two_item_list()
        {
            var item1 = FakeSyntax.Expression();
            var item2 = FakeSyntax.Expression();
            var tokens = FakeTokenStream.From($"{item1}{item2}");

            var l = ParseListWithoutError<CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(item2, i));
        }

        [Fact]
        public void Three_item_list()
        {
            var item1 = FakeSyntax.Expression();
            var item2 = FakeSyntax.Expression();
            var item3 = FakeSyntax.Expression();
            var tokens = FakeTokenStream.From($"{item1}{item2}{item3}");

            var l = ParseListWithoutError<CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(item2, i),
                i => Assert.Equal(item3, i));
        }

        [Fact]
        public void Unexpected_token_in_list()
        {
            var tokens = FakeTokenStream.From($"self");
            var parameter = FakeSyntax.Parameter();
            var expressionParser = FakeParser.Skip(parameter);
            var diagnostics = new DiagnosticsBuilder();

            var l = new ListParser().ParseList(tokens, expressionParser.Parse, TypeOf<CloseParenToken>(), diagnostics);

            Assert.Collection(l, i => Assert.Equal(parameter, i));
            Assert.Collection(diagnostics.Build(), d => { d.AssertParsingDiagnostic(3002, 0, 4); });
        }

        [Fact]
        public void Empty_separated_list()
        {
            var tokens = FakeTokenStream.From($"");
            var diagnostics = new DiagnosticsBuilder();

            var l = new ListParser().ParseSeparatedList(tokens, NotCalled, TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);

            Assert.Empty(diagnostics.Build());
            Assert.Empty(l);
        }

        [Fact]
        public void One_item_separated_list()
        {
            var item1 = FakeSyntax.Expression();
            var tokens = FakeTokenStream.From($"{item1}");

            var l = ParseSeparatedListWithoutError<CommaToken, CloseParenToken>(tokens);

            Assert.Collection(l, i => Assert.Equal(item1, i));
        }

        [Fact]
        public void Two_item_separated_list()
        {
            var item1 = FakeSyntax.Expression();
            var item2 = FakeSyntax.Expression();
            var tokens = FakeTokenStream.From($"{item1},{item2}");

            var l = ParseSeparatedListWithoutError<CommaToken, CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(tokens[1], i),
                i => Assert.Equal(item2, i));
        }

        [Fact]
        public void Three_item_separated_list()
        {
            var item1 = FakeSyntax.Expression();
            var item2 = FakeSyntax.Expression();
            var item3 = FakeSyntax.Expression();
            var tokens = FakeTokenStream.From($"{item1},{item2},{item3}");

            var l = ParseSeparatedListWithoutError<CommaToken, CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(tokens[1], i),
                i => Assert.Equal(item2, i),
                i => Assert.Equal(tokens[3], i),
                i => Assert.Equal(item3, i));
        }

        [Fact]
        public void Unexpected_token_in_separated_list()
        {
            var tokens = FakeTokenStream.From($"self");
            var parameter = FakeSyntax.Parameter();
            var expressionParser = FakeParser.Skip(parameter);
            var diagnostics = new DiagnosticsBuilder();

            var l = new ListParser().ParseSeparatedList(tokens, expressionParser.Parse, TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);

            Assert.Collection(l, i => Assert.Equal(parameter, i));
            Assert.Collection(diagnostics.Build(), d => { d.AssertParsingDiagnostic(3002, 0, 4); });
        }

        private static SyntaxNode NotCalled([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            Assert.True(false, "ParseFunction<T> called when it wasn't supposed to be");
            throw new Exception("Unreachable");
        }

        [NotNull]
        private static SyntaxList<ExpressionSyntax> ParseListWithoutError<TTerminator>([NotNull] ITokenStream tokens)
            where TTerminator : Token
        {
            var diagnostics = new DiagnosticsBuilder();
            var expressionParser = FakeParser.ForExpressions();
            var syntaxList = new ListParser().ParseList(tokens, expressionParser.ParseExpression, TypeOf<TTerminator>(), diagnostics);
            Assert.Empty(diagnostics.Build());
            return syntaxList;
        }

        [NotNull]
        private static SeparatedListSyntax<ExpressionSyntax> ParseSeparatedListWithoutError<TSeparator, TTerminator>([NotNull] ITokenStream tokens)
            where TSeparator : Token
            where TTerminator : Token
        {
            var diagnostics = new DiagnosticsBuilder();
            var expressionParser = FakeParser.ForExpressions();
            var l = new ListParser().ParseSeparatedList(tokens, expressionParser.ParseExpression, TypeOf<TSeparator>(), TypeOf<TTerminator>(), diagnostics);
            Assert.Empty(diagnostics.Build());
            return l;
        }
    }
}
