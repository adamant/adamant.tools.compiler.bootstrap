using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    // A factory for fakes
    public static class Fake
    {
        [NotNull]
        public static AccessModifierSyntax AccessModifier()
        {
            return new AccessModifierSyntax(null);
        }

        [NotNull]
        public static BlockStatementSyntax BlockStatement()
        {
            return new BlockStatementSyntax(null, SyntaxList<StatementSyntax>(), null);
        }

        [NotNull]
        public static SyntaxList<T> SyntaxList<T>()
            where T : SyntaxNode
        {
            return new SyntaxList<T>(Enumerable.Empty<T>());
        }

        [NotNull]
        public static ExpressionSyntax Expression()
        {
            return new FakeExpressionSyntax();
        }

        [NotNull]
        public static SeparatedListSyntax<T> SeparatedList<T>()
            where T : SyntaxNode
        {
            return new SeparatedListSyntax<T>(Enumerable.Empty<T>());
        }

        [NotNull]
        public static IListParser ListParser()
        {
            return new FakeListParser();
        }

        [NotNull]
        public static IParser<T> Parser<T>()
            where T : SyntaxNode
        {
            if (typeof(T) == typeof(UsingDirectiveSyntax))
                return (IParser<T>)new FakeUsingDirectiveParser();
            return new FakeParser<T>();
        }

        [NotNull]
        public static IParser<T> SkipParser<T>(T value)
            where T : SyntaxNode
        {
            return new FakeSkippedParser<T>(value);
        }

        [NotNull]
        public static NameSyntax Name()
        {
            return new FakeNameSyntax();
        }

        [NotNull]
        public static UsingDirectiveSyntax UsingDirective()
        {
            return new UsingDirectiveSyntax(null, Name(), null);
        }

        private class FakeExpressionSyntax : ExpressionSyntax
        {
        }

        private class FakeNameSyntax : NameSyntax
        {
        }

        private class FakeParser<T> : IParser<T>
            where T : SyntaxNode
        {
            public T Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
            {
                var fakeToken = tokens.ExpectFake();
                return (T)fakeToken?.FakeNode ?? throw new Exception($"Expected fake '{typeof(T).Name}' not found");
            }
        }

        private class FakeSkippedParser<T> : IParser<T>
            where T : SyntaxNode
        {
            private T value;

            public FakeSkippedParser(T value)
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

        private class FakeListParser : IListParser
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

        private class FakeUsingDirectiveParser : IParser<UsingDirectiveSyntax>
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

        public static ParameterSyntax Parameter()
        {
            return new ParameterSyntax(null, null, null, Fake.Expression());
        }
    }
}
