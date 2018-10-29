using System;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes
{
    public static class FakeParser
    {
        [NotNull]
        public static IListParser ForLists()
        {
            return new ListParser();
        }

        [NotNull]
        public static IExpressionParser ForExpressions()
        {
            return new ExpressionParser();
        }


        [NotNull]
        public static IParser<T> For<T>()
            where T : SyntaxNode
        {
            if (typeof(T) == typeof(UsingDirectiveSyntax))
                return (IParser<T>)new UsingDirectiveParser();
            if (typeof(T) == typeof(ModifierSyntax))
                return (IParser<T>)new ModifierParser();
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
                return (T)fakeToken?.FakeValue ?? throw new Exception($"Expected fake '{typeof(T).Name}' not found");
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
                Type<TTerminator> terminatorType,
                IDiagnosticsCollector diagnostics) where T : SyntaxNode where TTerminator : Token
            {
                var fakeToken = tokens.ExpectFake();
                return (SyntaxList<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }

            public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
                ITokenStream tokens,
                ParseFunction<T> parseItem,
                Type<TSeparator> separatorType,
                Type<TTerminator> terminatorType,
                IDiagnosticsCollector diagnostics) where T : SyntaxNode where TSeparator : Token where TTerminator : Token
            {
                var fakeToken = tokens.ExpectFake();
                return (SeparatedListSyntax<T>)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }
        }

        private class ExpressionParser : IExpressionParser
        {
            [NotNull]
            private readonly FakeTokenParser<ExpressionSyntax> fakeParser = new FakeTokenParser<ExpressionSyntax>();

            public ExpressionSyntax Parse(ITokenStream tokens, IDiagnosticsCollector diagnostics)
            {
                return fakeParser.Parse(tokens, diagnostics);
            }

            public ExpressionSyntax Parse(
                ITokenStream tokens,
                IDiagnosticsCollector diagnostics,
                OperatorPrecedence minPrecedence)
            {
                return fakeParser.Parse(tokens, diagnostics);
            }

            public SeparatedListSyntax<ArgumentSyntax> ParseArguments(ITokenStream tokens, IDiagnosticsCollector diagnostics)
            {
                return SeparatedListSyntax<ArgumentSyntax>.Empty;
            }
        }

        private class UsingDirectiveParser : IParser<UsingDirectiveSyntax>
        {
            [NotNull]
            public UsingDirectiveSyntax Parse(
                [NotNull] ITokenStream tokens,
                IDiagnosticsCollector diagnostics)
            {
                var _ = tokens.Take<UsingKeywordToken>();

                var fakeToken = tokens.ExpectFake();
                return (UsingDirectiveSyntax)fakeToken?.FakeValue ?? throw new InvalidOperationException();
            }
        }

        private class ModifierParser : IParser<ModifierSyntax>
        {
            public ModifierSyntax Parse(ITokenStream tokens, IDiagnosticsCollector diagnostics)
            {
                if (tokens.Current is FakeToken fake && fake.FakeValue is ModifierSyntax syntax)
                {
                    tokens.MoveNext();
                    return syntax;
                }

                return null;
            }
        }
    }
}
