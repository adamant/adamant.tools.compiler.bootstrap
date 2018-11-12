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
    public class ListParser : Parser, IListParser
    {
        public ListParser([NotNull] ITokenIterator tokens)
            : base(tokens)
        {
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<T> ParseList<T>([NotNull] Func<T> acceptItem)
            where T : class
        {
            return new Generator<T>(acceptItem)
                .TakeWhile(t => t != null).ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<T> ParseSeparatedList<T, TSeparator>([NotNull] Func<T> acceptItem)
            where T : class
            where TSeparator : class, IToken
        {
            var item = acceptItem();
            if (item == null)
                throw new ParseFailedException();
            var items = new List<T>();
            while (item != null)
            {
                items.Add(item);
                if (Tokens.AcceptToken<TSeparator>() == null)
                    break;

                item = acceptItem();
            }
            return items.ToFixedList();
        }

        [Obsolete("Use ParseList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        public FixedList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TTerminator : class, IToken
        {
            return ParseEnumerable(tokens, parseItem, terminatorType, diagnostics).ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<T> ParseEnumerable<T, TTerminator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TTerminator : class, IToken
        {
            while (!(tokens.Current is TTerminator || tokens.Current is IEndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens, diagnostics);
                if (tokens.Current == start)
                {
                    diagnostics.Add(ParseError.UnexpectedToken(tokens.Context.File, tokens.Current.Span));
                    tokens.Next();
                }
            }
        }

        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            Type<TSeparator> separatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TSeparator : class, IToken
        {
            return new SeparatedListSyntax<T>(ParseSeparatedEnumerable(tokens, acceptItem, separatorType, diagnostics));
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<ISyntaxNodeOrTokenPlace> ParseSeparatedEnumerable<TSeparator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] AcceptFunction<NonTerminal> acceptItem,
            Type<TSeparator> separatorType,
            [NotNull] Diagnostics diagnostics)
            where TSeparator : class, IToken
        {
            var item = acceptItem(tokens, diagnostics);
            if (item != null) yield return item;
            TSeparator separator;
            do
            {
                separator = tokens.AcceptToken<TSeparator>();
                if (separator != null) yield return separator;
                item = acceptItem(tokens, diagnostics);
                if (item != null) yield return item;
            } while (separator != null || item != null);
        }

        [Obsolete("Use ParseSeparatedList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TSeparator : class, IToken
            where TTerminator : class, IToken
        {
            return new SeparatedListSyntax<T>(ParseSeparatedEnumerable(tokens, parseItem, separatorType, terminatorType, diagnostics));
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<ISyntaxNodeOrTokenPlace> ParseSeparatedEnumerable<TSeparator, TTerminator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] ParseFunction<NonTerminal> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] Diagnostics diagnostics)
            where TSeparator : class, IToken
            where TTerminator : class, IToken
        {
            while (!(tokens.Current is TTerminator || tokens.Current is IEndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens, diagnostics);
                if (tokens.Current == start)
                {
                    diagnostics.Add(ParseError.UnexpectedToken(tokens.Context.File, tokens.Current.Span));
                    tokens.Next();
                }

                if (tokens.Current is TSeparator)
                    yield return tokens.Consume<TSeparator>();
                else
                    yield break;
            }
        }
    }
}
