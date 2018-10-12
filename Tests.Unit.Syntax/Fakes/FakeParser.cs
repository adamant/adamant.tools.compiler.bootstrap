using System;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public static class FakeParser
    {
        [NotNull]
        public static IListParser ForLists()
        {
            return new ListParser();
        }

        [NotNull]
        public static IParser<T> For<T>()
            where T : SyntaxNode
        {
            if (typeof(T) == typeof(UsingDirectiveSyntax))
                return (IParser<T>)new UsingDirectiveParser();
            return new FakeTokenParser<T>();
        }

        [NotNull]
        public static IParser<T> Skip<T>(T value)
            where T : SyntaxNode
        {
            return new SkipParser<T>(value);
        }

        private class FakeTokenParser<T> : IParser<T>
            where T : SyntaxNode
        {
            public T Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                var fakeToken = tokens.ExpectFake();
                return (T)fakeToken?.FakeNode ?? throw new Exception($"Expected fake '{typeof(T).Name}' not found");
            }
        }

        private class SkipParser<T> : IParser<T>
            where T : SyntaxNode
        {
            private T value;

            public SkipParser(T value)
            {
                this.value = value;
            }

            public T Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                var v = value;
                value = null;
                return v;
            }
        }

        private class ListParser : IListParser
        {
            public SyntaxList<T> ParseList<T, TTerminator>(
                ITokenStream tokens,
                ParseFunction<T> parseItem,
                TypeOf<TTerminator> terminatorType,
                IDiagnosticsCollector diagnostics) where T : SyntaxNode where TTerminator : Token
            {
                var fakeToken = tokens.ExpectFake();
                return (SyntaxList<T>)fakeToken?.FakeNode ?? throw new InvalidOperationException();
            }

            public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
                ITokenStream tokens,
                ParseFunction<T> parseItem,
                TypeOf<TSeparator> separatorType,
                TypeOf<TTerminator> terminatorType,
                IDiagnosticsCollector diagnostics) where T : SyntaxNode where TSeparator : Token where TTerminator : Token
            {
                var fakeToken = tokens.ExpectFake();
                return (SeparatedListSyntax<T>)fakeToken?.FakeNode ?? throw new InvalidOperationException();
            }
        }

        private class UsingDirectiveParser : IParser<UsingDirectiveSyntax>
        {
            [NotNull]
            public UsingDirectiveSyntax Parse(
                [NotNull] ITokenStream tokens,
                IDiagnosticsCollector diagnostics)
            {
                var _ = tokens.Expect<UsingKeywordToken>();

                var fakeToken = tokens.ExpectFake();
                return (UsingDirectiveSyntax)fakeToken?.FakeNode ?? throw new InvalidOperationException();
            }
        }
    }
}
