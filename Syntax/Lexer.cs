using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Lexer : ILexer
    {
        public static IReadOnlyDictionary<string, TokenKind> Keywords;

        static Lexer()
        {
            var keywordDictionary = new Dictionary<string, TokenKind>();
            var keywords = Enum.GetValues(typeof(TokenKind))
                .Cast<TokenKind>()
                .Where(t => t.ToString().EndsWith("Keyword"));
            var keywordLength = "Keyword".Length;
            foreach (var keyword in keywords)
            {
                var enumName = keyword.ToString();
                var text = enumName
                    .Substring(0, enumName.Length - keywordLength)
                    .ToLower();
                keywordDictionary.Add(text, keyword);
            }

            Keywords = new ReadOnlyDictionary<string, TokenKind>(keywordDictionary);
        }

        public IEnumerable<Token> Lex(CodeText code)
        {
            var tokenStart = 0;
            var tokenEnd = -1; // One past the end position to allow for zero length spans
            var tokenDiagnosticInfos = new List<DiagnosticInfo>();
            while (tokenStart < code.Length)
            {
                var currentChar = code[tokenStart];
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
                            while (tokenStart < code.Length && code[tokenStart] != '\r' && code[tokenStart] != '\n')
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
                            while (tokenStart < code.Length && !(lastCharWasStar && code[tokenStart] == '/'))
                            {
                                lastCharWasStar = code[tokenStart] == '*';
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
                    case '"':
                        yield return LexString();
                        break;
                    default:
                        if (IsIdentiferStartCharacter(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < code.Length && IsIdentifierCharacter(code[tokenEnd]))
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

                var token = Token.New(code, TextSpan.FromStartEnd(tokenStart, tokenEnd), kind, tokenDiagnosticInfos);
                tokenDiagnosticInfos.Clear();
                return token;
            }

            Token NewIdentifierOrKeywordToken()
            {
                var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
                var value = code[span];
                Token token;
                if (Keywords.TryGetValue(value, out TokenKind keywordKind))
                {
                    token = Token.New(code, span, keywordKind, tokenDiagnosticInfos);
                }
                else
                    token = new IdentifierToken(code, span, IdentifierKind.Normal, value, tokenDiagnosticInfos);

                tokenDiagnosticInfos.Clear();
                return token;
            }

            bool NextCharIs(char c)
            {
                var index = tokenStart + 1;
                return index < code.Length && code[index] == c;
            }

            bool CharIs(int i, char c)
            {
                var index = tokenStart + i;
                return index < code.Length && code[index] == c;
            }

            void NewError(DiagnosticLevel level, string message)
            {
                tokenDiagnosticInfos.Add(new DiagnosticInfo(level, DiagnosticPhase.Lexing, message));
            }

            StringLiteralToken LexString()
            {
                tokenEnd = tokenStart + 1;
                var content = new StringBuilder();
                char currentChar;
                while (tokenEnd < code.Length && (currentChar = code[tokenEnd]) != '"')
                {
                    // if Escape Sequence
                    if (currentChar == '\\')
                    {
                        tokenEnd += 1;
                        if (tokenEnd >= code.Length)
                        {
                            content.Append(currentChar);
                            NewError(DiagnosticLevel.CompilationError, "Invalid string escape sequence `\\EOF`.");
                            break; // we hit the end of file and need to not add to tokenEnd any more
                        }
                        else
                        {
                            currentChar = code[tokenEnd];
                            switch (currentChar)
                            {
                                case '"':
                                case '\\':
                                    content.Append(currentChar);
                                    break;
                                case 'n':
                                    content.Append('\n');
                                    break;
                                case 'r':
                                    content.Append('\r');
                                    break;
                                case '0':
                                    content.Append('\0');
                                    break;
                                case 't':
                                    content.Append('\t');
                                    break;
                                case 'u':
                                    tokenEnd += 1; // consume the 'u'
                                    if (tokenEnd < code.Length && code[tokenEnd] == '(')
                                        tokenEnd += 1;
                                    else
                                        NewError(DiagnosticLevel.CompilationError, $"Unicode escape sequence must begin `\\u(` rather than `\\{code[tokenEnd]}`.");

                                    var codepoint = new StringBuilder(6);
                                    while (tokenEnd < code.Length && IsHexDigit(currentChar = code[tokenEnd]))
                                    {
                                        codepoint.Append(currentChar);
                                        tokenEnd += 1;
                                    }

                                    content.Append(char.ConvertFromUtf32(Convert.ToInt32(codepoint.ToString(), 16)));

                                    if (tokenEnd >= code.Length)
                                    {
                                        NewError(DiagnosticLevel.CompilationError, $"Unclosed Unicode escape sequence.");
                                        break;
                                    }
                                    else if (tokenEnd < code.Length && code[tokenEnd] == ')')
                                        tokenEnd += 1;
                                    else
                                        NewError(DiagnosticLevel.CompilationError, $"Unicode escape sequence must be terminated with `)`.");

                                    // We have already consumed all the characters of the escape sequence and now want to continue the loop
                                    continue;
                                default:
                                    NewError(DiagnosticLevel.CompilationError, $"Invalid string escape sequence `\\{currentChar}`.");
                                    content.Append('\\');
                                    content.Append(currentChar);
                                    break;
                            }
                        }
                    }
                    else
                        content.Append(currentChar);

                    tokenEnd += 1;
                }

                // To include the close quote
                if (tokenEnd < code.Length && code[tokenEnd] == '"')
                    tokenEnd += 1;
                var token = new StringLiteralToken(code, TextSpan.FromStartEnd(tokenStart, tokenEnd), content.ToString(), tokenDiagnosticInfos);
                tokenDiagnosticInfos.Clear();
                return token;
            }
        }


        private static bool IsIdentiferStartCharacter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private static bool IsIdentifierCharacter(char c)
        {

            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || char.IsNumber(c);
        }

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9')
                || (c >= 'a' && c <= 'f')
                || (c >= 'A' && c <= 'F');
        }
    }
}
