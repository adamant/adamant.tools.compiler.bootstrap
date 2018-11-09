using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Fakes
{
    public class FakeTokenStream : ITokenStream
    {
        [NotNull]
        public CodeFile File { get; }

        [NotNull]
        public IReadOnlyList<Token> Tokens { get; }

        [NotNull]
        private readonly TokenStream stream;

        public FakeTokenStream([NotNull] CodeFile file, [NotNull][ItemNotNull] IEnumerable<Token> tokens)
        {
            File = file;
            Tokens = tokens.ToReadOnlyList();
            stream = new TokenStream(file, Tokens);
        }

        public IToken this[int index] => Tokens[index];

        [NotNull]
        public static FakeTokenStream From([NotNull] FormattableString tokenDescription)
        {
            var file = tokenDescription.Format.AssertNotNull().ToFakeCodeFile();
            var tokens = CreateFakeTokens(new Lexer().Lex(file), tokenDescription.GetArguments().AssertNotNull());
            return new FakeTokenStream(file, tokens);
        }

        [NotNull]
        public static FakeTokenStream FromString([NotNull] string tokenDescription)
        {
            var file = tokenDescription.ToFakeCodeFile();
            var tokens = new Lexer().Lex(file).Where(t => !(t is TriviaToken));
            return new FakeTokenStream(file, tokens);
        }

        [NotNull]
        public static IEnumerable<Token> CreateFakeTokens(
            [NotNull][ItemNotNull] IEnumerable<Token> tokens,
            [NotNull] IReadOnlyList<object> fakeTokenValues)
        {
            using (var enumerator = tokens.GetEnumerator().AssertNotNull())
                while (enumerator.MoveNext())
                {
                    switch (enumerator.Current)
                    {
                        case OpenBraceToken _:
                            var startSpan = enumerator.Current.Span;
                            Assert.True(enumerator.MoveNext());
                            if (enumerator.Current is OpenBraceToken)
                            {
                                // Escaped open brace
                                yield return enumerator.Current;
                            }
                            else
                            {
                                var placeholder = (int)Assert.IsType<IntegerLiteralToken>(enumerator.Current).Value;
                                Assert.True(enumerator.MoveNext());
                                Assert.IsType<CloseBraceToken>(enumerator.Current);
                                var value = fakeTokenValues[placeholder];
                                yield return new FakeToken(TextSpan.Covering(startSpan, enumerator.Current.Span), value);
                            }
                            break;
                        case CloseBraceToken _:
                            // Escaped close brace
                            Assert.True(enumerator.MoveNext());
                            Assert.IsType<CloseBraceToken>(enumerator.Current);
                            yield return enumerator.Current;
                            break;
                        case TriviaToken _:
                            // Skip
                            break;
                        default:
                            yield return enumerator.Current;
                            break;
                    }
                }
        }

        #region Forwards
        public bool MoveNext()
        {
            return stream.MoveNext();
        }

        public void Reset()
        {
            stream.Reset();
        }

        public Token Current => stream.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            stream.Dispose();
        }
        #endregion
    }
}
