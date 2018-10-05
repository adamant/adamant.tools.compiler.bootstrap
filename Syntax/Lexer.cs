using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Lexer
    {
        [MustUseReturnValue]
        public IEnumerable<Token> Lex(CodeFile file)
        {
            var code = file.Code;
            var text = code.Text;
            var tokenStart = 0;
            var tokenEnd = -1; // One past the end position to allow for zero length spans
            var diagnostics = new List<Diagnostic>();
            while (tokenStart < text.Length)
            {
                var currentChar = text[tokenStart];
                switch (currentChar)
                {
                    case '{':
                        yield return NewOperatorToken(TokenKind.OpenBrace);
                        break;
                    case '}':
                        yield return NewOperatorToken(TokenKind.CloseBrace);
                        break;
                    case '(':
                        yield return NewOperatorToken(TokenKind.OpenParen);
                        break;
                    case ')':
                        yield return NewOperatorToken(TokenKind.CloseParen);
                        break;
                    case '[':
                        yield return NewOperatorToken(TokenKind.OpenBracket);
                        break;
                    case ']':
                        yield return NewOperatorToken(TokenKind.CloseBracket);
                        break;
                    case ';':
                        yield return NewOperatorToken(TokenKind.Semicolon);
                        break;
                    case ',':
                        yield return NewOperatorToken(TokenKind.Comma);
                        break;
                    case '.':
                        if (NextCharIs('.'))
                        {
                            // it is `..`
                            yield return NewOperatorToken(TokenKind.DotDot, 2);
                        }
                        else
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
                    case '$':
                        yield return NewOperatorToken(TokenKind.Dollar);
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
                        switch (NextChar())
                        {
                            case '=':
                                // it is `-=`
                                yield return NewOperatorToken(TokenKind.MinusEquals, 2);
                                break;
                            case '>':
                                // it is `->`
                                yield return NewOperatorToken(TokenKind.RightArrow, 2);
                                break;
                            default:
                                // it is `-`
                                yield return NewOperatorToken(TokenKind.Minus);
                                break;
                        }
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
                        switch (NextChar())
                        {
                            case '/':
                                // it is a line comment `//`
                                tokenEnd = tokenStart + 2;
                                while (tokenEnd < text.Length && text[tokenEnd] != '\r' && text[tokenEnd] != '\n')
                                    tokenEnd += 1;

                                yield return NewToken(TokenKind.Comment, tokenEnd);
                                break;
                            case '*':
                                // it is a block comment `/*`
                                tokenEnd = tokenStart + 2;
                                var lastCharWasStar = false;
                                while (tokenEnd < text.Length && !(lastCharWasStar && text[tokenEnd] == '/'))
                                {
                                    lastCharWasStar = text[tokenEnd] == '*';
                                    tokenEnd += 1;
                                }
                                // If we didn't run into the end of the file, we need to move past the file '/'
                                if (tokenEnd < text.Length)
                                    tokenEnd += 1;
                                yield return NewToken(TokenKind.Comment, tokenEnd);
                                break;
                            case '=':
                                // it is `/=`
                                yield return NewOperatorToken(TokenKind.SlashEquals, 2);
                                break;
                            default:
                                // it is `/`
                                yield return NewOperatorToken(TokenKind.Slash);
                                break;
                        }
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
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        tokenEnd = tokenStart + 1;
                        while (tokenEnd < text.Length && IsIntegerCharacter(text[tokenEnd]))
                            tokenEnd += 1;

                        var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
                        var value = BigInteger.Parse(code[span]);
                        yield return new Token(TokenKind.IntegerLiteral, span, value);
                        break;
                    default:
                        if (char.IsWhiteSpace(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < text.Length && char.IsWhiteSpace(text[tokenEnd]))
                                tokenEnd += 1;

                            yield return NewToken(TokenKind.Whitespace, tokenEnd);
                        }
                        else if (IsIdentifierStartCharacter(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < text.Length && IsIdentifierCharacter(text[tokenEnd]))
                                tokenEnd += 1;

                            yield return NewIdentifierOrKeywordToken();
                        }
                        else if (currentChar == '!' && NextCharIs('='))
                        {
                            NewError(DiagnosticLevel.CompilationError, "Use `≠` or `=/=` for not equal instead of `!=`.");
                            yield return NewOperatorToken(TokenKind.NotEqual, 2);
                        }
                        else
                        {
                            NewError(DiagnosticLevel.CompilationError, "Unexpected Character");
                            yield return NewToken(TokenKind.Unexpected, tokenStart + 1);
                        }
                        break;
                }
                tokenStart = tokenEnd;
            }

            // The end of file token provides something to attach any final errors to
            yield return NewToken(TokenKind.EndOfFile, tokenStart, diagnostics.AsReadOnly());
            yield break;

            Token NewOperatorToken(TokenKind kind, int length = 1)
            {
                return NewToken(kind, tokenStart + length);
            }

            Token NewToken(TokenKind kind, int? end = null, object value = null)
            {
                // If we were given an end value, set tokenEnd correctly
                tokenEnd = end ?? tokenEnd;

                return new Token(kind, TextSpan.FromStartEnd(tokenStart, tokenEnd), value);
            }

            Token NewIdentifierOrKeywordToken()
            {
                var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
                var value = code[span];
                if (Keywords.Map.TryGetValue(value, out var keywordKind))
                    return new Token(keywordKind, span);

                return new Token(TokenKind.Identifier, span, value);
            }

            char? NextChar()
            {
                var index = tokenStart + 1;
                return index < text.Length ? text[index] : default;
            }

            // TODO replace all uses of this with `switch(NextChar())`
            bool NextCharIs(char c)
            {
                var index = tokenStart + 1;
                return index < text.Length && text[index] == c;
            }

            bool CharIs(int i, char c)
            {
                var index = tokenStart + i;
                return index < text.Length && text[index] == c;
            }

            void NewError(DiagnosticLevel level, string message)
            {
                // TODO report correct span
                diagnostics.Add(SyntaxError.LexError(file, new TextSpan(tokenStart, 0), level, message));
            }

            Token LexString()
            {
                tokenEnd = tokenStart + 1;
                var content = new StringBuilder();
                char currentChar;
                while (tokenEnd < text.Length && (currentChar = text[tokenEnd]) != '"')
                {
                    tokenEnd += 1;

                    if (currentChar != '\\')
                    {
                        content.Append(currentChar);
                        continue;
                    }

                    // Escape Sequence (i.e. "\\")

                    if (tokenEnd >= text.Length)
                    {
                        content.Append(currentChar);
                        NewError(DiagnosticLevel.CompilationError, "Invalid string escape sequence `\\EOF`.");
                        break; // we hit the end of file and need to not add to tokenEnd any more
                    }

                    // Escape Sequence with next char (i.e. "\\x")
                    currentChar = text[tokenEnd];
                    tokenEnd += 1;
                    switch (currentChar)
                    {
                        case '"':
                        case '\'':
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
                            if (tokenEnd >= text.Length)
                            {
                                NewError(DiagnosticLevel.CompilationError,
                                    "Unclosed Unicode escape sequence.");
                                break;
                            }
                            else if (text[tokenEnd] == '(')
                                tokenEnd += 1;
                            else
                            {
                                NewError(DiagnosticLevel.CompilationError,
                                    $"Unicode escape sequence must begin `\\u(` rather than `\\u{text[tokenEnd]}`.");
                                break;
                            }

                            var codepoint = new StringBuilder(6);
                            while (tokenEnd < text.Length &&
                                   IsHexDigit(currentChar = text[tokenEnd]))
                            {
                                codepoint.Append(currentChar);
                                tokenEnd += 1;
                            }

                            content.Append(
                                char.ConvertFromUtf32(Convert.ToInt32(codepoint.ToString(),
                                    16)));

                            if (tokenEnd >= text.Length)
                                NewError(DiagnosticLevel.CompilationError,
                                    "Unclosed Unicode escape sequence.");
                            else if (text[tokenEnd] == ')')
                                tokenEnd += 1;
                            else
                                NewError(DiagnosticLevel.CompilationError,
                                    "Unicode escape sequence must be terminated with `)`.");
                            break;
                        default:
                            NewError(DiagnosticLevel.CompilationError,
                                $"Invalid string escape sequence `\\{currentChar}`.");
                            content.Append('\\');
                            content.Append(currentChar);
                            break;
                    }
                }

                // To include the close quote
                if (tokenEnd < text.Length && text[tokenEnd] == '"')
                    tokenEnd += 1;

                return new Token(TokenKind.StringLiteral, TextSpan.FromStartEnd(tokenStart, tokenEnd), content.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MustUseReturnValue]
        private static bool IsIntegerCharacter(char c)
        {
            return c >= '0' && c <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MustUseReturnValue]
        private static bool IsIdentifierStartCharacter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MustUseReturnValue]
        private static bool IsIdentifierCharacter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || char.IsNumber(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MustUseReturnValue]
        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9')
                || (c >= 'a' && c <= 'f')
                || (c >= 'A' && c <= 'F');
        }
    }
}
