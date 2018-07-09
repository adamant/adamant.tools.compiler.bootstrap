using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Lexer : IEnumerable<Token>
    {
        private readonly SourceText source;

        public Lexer(SourceText source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public IEnumerator<Token> GetEnumerator()
        {
            var tokenStart = 0;
            var tokenEnd = -1; // One past the end position to allow for zero length spans
            var tokenDiagnosticInfos = new List<DiagnosticInfo>();

            while (tokenStart < source.Length)
            {
                var currentChar = source[tokenStart];
                switch (currentChar)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        tokenStart += 1;
                        continue;
                    case '{':
                        yield return NewOperatorToken(TokenKind.LeftBrace);
                        break;
                    case '+':
                        if (tokenStart + 1 < source.Length && source[tokenStart] == '=')
                            // it is `+=`
                            yield return NewOperatorToken(TokenKind.PlusEquals, 2);
                        else
                            // it is `+`
                            yield return NewOperatorToken(TokenKind.Plus);
                        break;
                    default:
                        if (IsIdentiferStartCharacter(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < source.Length && IsIdentifierCharacter(source[tokenEnd]))
                                tokenEnd += 1;


                            yield return NewIdentifierOrKeywordToken();
                        }
                        throw new NotImplementedException();
                }
                tokenStart = tokenEnd;
            }

            // The end of file token provides something to attach any final errors to
            yield return NewToken(TokenKind.EndOfFile, tokenStart);

            Token NewOperatorToken(TokenKind kind, int length = 1)
            {
                return NewToken(kind, tokenStart + length);
            }

            Token NewToken(TokenKind kind, int? end = null)
            {
                // If we were given an end value, set tokenEnd correctly
                tokenEnd = end ?? tokenEnd;

                var token = Token.New(source, TextSpan.FromStartEnd(tokenStart, tokenEnd), kind, tokenDiagnosticInfos);
                tokenDiagnosticInfos.Clear();
                return token;
            }

            Token NewIdentifierOrKeywordToken()
            {
                // TODO check for keywords
                var kind = TokenKind.Identifier;
                return NewToken(kind); // TODO Identifiers should have a string value because of escaped identifiers?
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static bool IsIdentiferStartCharacter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private static bool IsIdentifierCharacter(char c)
        {

            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || char.IsNumber(c);
        }
    }
}
