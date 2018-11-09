using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class ListParser : IListParser
    {
        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<T> ParseList<T>(
            [NotNull] ITokenStream tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : NonTerminal
        {
            return new Generator<T>(() => acceptItem(tokens, diagnostics))
                .TakeWhile(t => t != null).ToSyntaxList();
        }

        [Obsolete("Use ParseList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : NonTerminal
            where TTerminator : class, IToken
        {
            return ParseEnumerable(tokens, parseItem, terminatorType, diagnostics).ToSyntaxList();
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<T> ParseEnumerable<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : NonTerminal
            where TTerminator : class, IToken
        {
            while (!(tokens.Current is TTerminator || tokens.Current is IEndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens, diagnostics);
                if (tokens.Current == start)
                {
                    diagnostics.Publish(ParseError.UnexpectedToken(tokens.File, tokens.Current.Span));
                    tokens.MoveNext();
                }
            }
        }

        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator>(
            [NotNull] ITokenStream tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            Type<TSeparator> separatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : NonTerminal
            where TSeparator : class, IToken
        {
            return new SeparatedListSyntax<T>(ParseSeparatedEnumerable(tokens, acceptItem, separatorType, diagnostics));
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<ISyntaxNodeOrTokenPlace> ParseSeparatedEnumerable<TSeparator>(
            [NotNull] ITokenStream tokens,
            [NotNull] AcceptFunction<NonTerminal> acceptItem,
            Type<TSeparator> separatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where TSeparator : class, IToken
        {
            var item = acceptItem(tokens, diagnostics);
            if (item != null) yield return item;
            TSeparator separator;
            do
            {
                separator = tokens.Accept<TSeparator>();
                if (separator != null) yield return separator;
                item = acceptItem(tokens, diagnostics);
                if (item != null) yield return item;
            } while (separator != null || item != null);
        }

        [Obsolete("Use ParseSeparatedList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : NonTerminal
            where TSeparator : class, IToken
            where TTerminator : class, IToken
        {
            return new SeparatedListSyntax<T>(ParseSeparatedEnumerable(tokens, parseItem, separatorType, terminatorType, diagnostics));
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<ISyntaxNodeOrTokenPlace> ParseSeparatedEnumerable<TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<NonTerminal> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where TSeparator : class, IToken
            where TTerminator : class, IToken
        {
            while (!(tokens.Current is TTerminator || tokens.Current is IEndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens, diagnostics);
                if (tokens.Current == start)
                {
                    diagnostics.Publish(ParseError.UnexpectedToken(tokens.File, tokens.Current.Span));
                    tokens.MoveNext();
                }

                if (tokens.Current is TSeparator)
                    yield return tokens.Expect<TSeparator>();
                else
                    yield break;
            }
        }
    }
}
