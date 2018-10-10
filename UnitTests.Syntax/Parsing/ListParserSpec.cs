using System;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Parsing
{
    [UnitTest]
    [Category("Parse")]
    public class ListParserSpec
    {
        [Fact]
        public void Empty_list()
        {
            var tokens = FakeTokenStream.From($"");

            var l = new ListParser().ParseList(tokens, NotCalled, TypeOf<CloseParenToken>._);

            Assert.Empty(l);
        }

        [Fact]
        public void One_item_list()
        {
            var item1 = Fake.Expression();
            var tokens = FakeTokenStream.From($"{item1}");

            var l = ParseListWithoutError<CloseParenToken>(tokens);

            Assert.Collection(l, i => Assert.Equal(item1, i));
        }

        [Fact]
        public void Two_item_list()
        {
            var item1 = Fake.Expression();
            var item2 = Fake.Expression();
            var tokens = FakeTokenStream.From($"{item1}{item2}");

            var l = ParseListWithoutError<CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(item2, i));
        }

        [Fact]
        public void Three_item_list()
        {
            var item1 = Fake.Expression();
            var item2 = Fake.Expression();
            var item3 = Fake.Expression();
            var tokens = FakeTokenStream.From($"{item1}{item2}{item3}");

            var l = ParseListWithoutError<CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(item2, i),
                i => Assert.Equal(item3, i));
        }

        [Fact]
        public void Empty_separated_list()
        {
            var tokens = FakeTokenStream.From($"");

            var l = new ListParser().ParseSeparatedList(tokens, NotCalled, TypeOf<CommaToken>._, TypeOf<CloseParenToken>._);

            Assert.Empty(l);
        }

        [Fact]
        public void One_item_separated_list()
        {
            var item1 = Fake.Expression();
            var tokens = FakeTokenStream.From($"{item1}");

            var l = ParseSeparatedListWithoutError<CommaToken, CloseParenToken>(tokens);

            Assert.Collection(l, i => Assert.Equal(item1, i));
        }

        [Fact]
        public void Two_item_separated_list()
        {
            var item1 = Fake.Expression();
            var item2 = Fake.Expression();
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
            var item1 = Fake.Expression();
            var item2 = Fake.Expression();
            var item3 = Fake.Expression();
            var tokens = FakeTokenStream.From($"{item1},{item2},{item3}");

            var l = ParseSeparatedListWithoutError<CommaToken, CloseParenToken>(tokens);

            Assert.Collection(l,
                i => Assert.Equal(item1, i),
                i => Assert.Equal(tokens[1], i),
                i => Assert.Equal(item2, i),
                i => Assert.Equal(tokens[3], i),
                i => Assert.Equal(item3, i));
        }

        private static SyntaxNode NotCalled([NotNull] ITokenStream tokens)
        {
            Assert.True(false, "ParseFunction<T> called when it wasn't supposed to be");
            throw new Exception("Unreachable");
        }

        [NotNull]
        private static SyntaxList<ExpressionSyntax> ParseListWithoutError<TTerminator>([NotNull] ITokenStream tokens)
            where TTerminator : Token
        {
            var diagnostics = new DiagnosticsBuilder();
            var syntaxList = new ListParser().ParseList(tokens, t => Fake.Parser<ExpressionSyntax>().Parse(t, diagnostics), TypeOf<TTerminator>._);
            Assert.Empty(diagnostics.Build());
            return syntaxList;
        }

        [NotNull]
        private static SeparatedListSyntax<ExpressionSyntax> ParseSeparatedListWithoutError<TSeparator, TTerminator>([NotNull] ITokenStream tokens)
            where TSeparator : Token
            where TTerminator : Token
        {
            var diagnostics = new DiagnosticsBuilder();
            var l = new ListParser().ParseSeparatedList(tokens,
                t => Fake.Parser<ExpressionSyntax>().Parse(t, diagnostics),
                TypeOf<TSeparator>._,
                TypeOf<TTerminator>._);
            Assert.Empty(diagnostics.Build());
            return l;
        }
    }
}
