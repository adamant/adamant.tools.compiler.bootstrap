using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Parsing.Fakes
{
    public class FakeTokenStream : ITokenIterator
    {
        [NotNull] public IReadOnlyList<IToken> Tokens { get; }
        [NotNull] private readonly ITokenIterator tokenIterator;

        public FakeTokenStream([NotNull] ParseContext context, [NotNull, ItemNotNull] IEnumerable<IToken> tokens)
        {
            Tokens = tokens.ToReadOnlyList();
            tokenIterator = new TokenIterator(context, Tokens);
        }

        public ITokenPlace this[int index] => Tokens[index];

        [NotNull]
        public static FakeTokenStream From([NotNull] FormattableString tokenDescription)
        {
            var file = FakeCodeFile.For(tokenDescription.Format.NotNull());
            var context = new ParseContext(file, new Diagnostics());
            var tokenIterator = new Lexer().Lex(context);
            var tokens = CreateFakeTokens(tokenIterator, tokenDescription.GetArguments().NotNull());
            return new FakeTokenStream(context, tokens);
        }

        [NotNull]
        public static FakeTokenStream FromString([NotNull] string tokenDescription)
        {
            var file = FakeCodeFile.For(tokenDescription);
            var context = new ParseContext(file, new Diagnostics());
            var tokenIterator = new Lexer().Lex(context).WhereNotTrivia();
            var tokens = CreateFakeTokens(tokenIterator, Enumerable.Empty<object>().ToReadOnlyList());
            return new FakeTokenStream(context, tokens);
        }

        [NotNull]
        public static IEnumerable<IToken> CreateFakeTokens(
            [NotNull] ITokenIterator tokens,
            [NotNull, ItemNotNull] IReadOnlyList<object> fakeTokenValues)
        {
            do
            {
                switch (tokens.Current)
                {
                    case IOpenBraceToken _:
                        var startSpan = tokens.Current.Span;
                        Assert.True(tokens.Next());
                        if (tokens.Current is IOpenBraceToken)
                        {
                            // Escaped open brace
                            yield return tokens.Current;
                        }
                        else
                        {
                            var placeholder = (int)tokens.Current
                                ?.AssertOfType<IIntegerLiteralToken>().Value;
                            Assert.True(tokens.Next());
                            tokens.Current.AssertOfType<ICloseBraceToken>();
                            var value = fakeTokenValues[placeholder];
                            yield return new FakeToken(
                                TextSpan.Covering(startSpan, tokens.Current.Span), value);
                        }
                        break;
                    case ICloseBraceToken _:
                        // Escaped close brace
                        Assert.True(tokens.Next());
                        tokens.Current.AssertOfType<ICloseBraceToken>();
                        yield return tokens.Current;
                        break;
                    case ITriviaToken _:
                        // Skip
                        break;
                    default:
                        yield return tokens.Current;
                        break;
                }
            } while (tokens.Next());
        }

        #region Forwards
        public ParseContext Context => tokenIterator.Context;

        public IToken Current => tokenIterator.Current;

        public bool Next()
        {
            return tokenIterator.Next();
        }
        #endregion
    }
}
