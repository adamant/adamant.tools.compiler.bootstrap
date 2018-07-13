using System;
using System.Collections;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

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
                    case '}':
                        yield return NewOperatorToken(TokenKind.RightBrace);
                        break;
                    case '(':
                        yield return NewOperatorToken(TokenKind.LeftParen);
                        break;
                    case ')':
                        yield return NewOperatorToken(TokenKind.RightParen);
                        break;
                    case '[':
                        yield return NewOperatorToken(TokenKind.LeftBracket);
                        break;
                    case ']':
                        yield return NewOperatorToken(TokenKind.RightBracket);
                        break;
                    case ';':
                        yield return NewOperatorToken(TokenKind.Semicolon);
                        break;
                    case ',':
                        yield return NewOperatorToken(TokenKind.Comma);
                        break;
                    case '.':
                        yield return NewOperatorToken(TokenKind.Dot);
                        break;
                    case ':':
                        yield return NewOperatorToken(TokenKind.Colon);
                        break;
                    case '?':
                        yield return NewOperatorToken(TokenKind.Question);
                        break;
                    case '|':
                        yield return NewOperatorToken(TokenKind.Pipe);
                        break;
                    case '→':
                        yield return NewOperatorToken(TokenKind.RightArrow);
                        break;
                    case '@':
                        yield return NewOperatorToken(TokenKind.AtSign);
                        break;
                    case '^':
                        yield return NewOperatorToken(TokenKind.Caret);
                        break;
                    case '+':
                        if (NextCharIs('='))
                            // it is `+=`
                            yield return NewOperatorToken(TokenKind.PlusEquals, 2);
                        else
                            // it is `+`
                            yield return NewOperatorToken(TokenKind.Plus);
                        break;
                    case '-':
                        if (NextCharIs('='))
                            // it is `-=`
                            yield return NewOperatorToken(TokenKind.MinusEquals, 2);
                        else if (NextCharIs('>'))
                            // it is `->`
                            yield return NewOperatorToken(TokenKind.RightArrow, 2);
                        else
                            // it is `-`
                            yield return NewOperatorToken(TokenKind.Minus);
                        break;
                    case '*':
                        if (NextCharIs('='))
                            // it is `*=`
                            yield return NewOperatorToken(TokenKind.AsteriskEquals, 2);
                        else
                            // it is `*`
                            yield return NewOperatorToken(TokenKind.Asterisk);
                        break;
                    case '/':
                        if (NextCharIs('/'))
                        {
                            // it is a line comment `//`
                            tokenStart += 2;
                            while (tokenStart < source.Length && source[tokenStart] != '\r' && source[tokenStart] != '\n')
                            {
                                tokenStart += 1;
                            }
                            continue;
                        }
                        else if (NextCharIs('*'))
                        {
                            // it is a block comment `/*`
                            tokenStart += 2;
                            var lastCharWasStar = false;
                            while (tokenStart < source.Length && !(lastCharWasStar && source[tokenStart] == '/'))
                            {
                                lastCharWasStar = source[tokenStart] == '*';
                                tokenStart += 1;
                            }
                            tokenStart += 1; // move past the final '/'
                            continue;
                        }
                        else if (NextCharIs('='))
                            // it is `/=`
                            yield return NewOperatorToken(TokenKind.SlashEquals, 2);
                        else
                            // it is `/`
                            yield return NewOperatorToken(TokenKind.Slash);
                        break;
                    case '=':
                        if (NextCharIs('='))
                            // it is `==`
                            yield return NewOperatorToken(TokenKind.EqualsEquals, 2);
                        else if (NextCharIs('/') && CharIs(2, '='))
                            // it is `=/=`
                            yield return NewOperatorToken(TokenKind.NotEqual, 3);
                        else
                            // it is `=`
                            yield return NewOperatorToken(TokenKind.Equals);
                        break;
                    case '≠':
                        yield return NewOperatorToken(TokenKind.NotEqual);
                        break;
                    case '>':
                        if (NextCharIs('='))
                            // it is `>=`
                            yield return NewOperatorToken(TokenKind.GreaterThanOrEqual, 2);
                        else
                            // it is `>`
                            yield return NewOperatorToken(TokenKind.GreaterThan);
                        break;
                    case '≥':
                    case '⩾':
                        yield return NewOperatorToken(TokenKind.GreaterThanOrEqual);
                        break;
                    case '<':
                        if (NextCharIs('='))
                            // it is `<=`
                            yield return NewOperatorToken(TokenKind.LessThanOrEqual, 2);
                        else
                            // it is `<`
                            yield return NewOperatorToken(TokenKind.LessThan);
                        break;
                    case '≤':
                    case '⩽':
                        yield return NewOperatorToken(TokenKind.LessThanOrEqual);
                        break;
                    default:
                        if (IsIdentiferStartCharacter(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < source.Length && IsIdentifierCharacter(source[tokenEnd]))
                                tokenEnd += 1;


                            yield return NewIdentifierOrKeywordToken();
                        }
                        else if (currentChar == '!' && NextCharIs('='))
                        {
                            NewError(DiagnosticLevel.CompilationError, "Use `≠` or `=/=` for not equal instead of `!=`.");
                            yield return NewOperatorToken(TokenKind.NotEqual, 2);
                        }
                        else
                            throw new NotImplementedException($"Current char=`{currentChar}`");
                        break;
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
                return NewToken(kind); // TODO create IdentifierToken with value, IsEscaped etc.
            }

            bool NextCharIs(char c)
            {
                var index = tokenStart + 1;
                return index < source.Length && source[index] == c;
            }

            bool CharIs(int i, char c)
            {
                var index = tokenStart + i;
                return index < source.Length && source[index] == c;
            }

            void NewError(DiagnosticLevel level, string message)
            {
                tokenDiagnosticInfos.Add(new DiagnosticInfo(level, DiagnosticPhase.Lexing, message));
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
