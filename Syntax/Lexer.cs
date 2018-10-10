using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Errors;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Lexer
    {
        [MustUseReturnValue]
        [NotNull]
        public IEnumerable<Token> Lex([NotNull] CodeFile file)
        {
            var code = file.Code;
            var text = code.Text;
            var tokenStart = 0;
            var tokenEnd = -1; // One past the end position to allow for zero length spans
            var diagnostics = new DiagnosticsBuilder();
            while (tokenStart < text.Length)
            {
                var currentChar = text[tokenStart];
                switch (currentChar)
                {
                    case '{':
                        yield return new OpenBraceToken(SymbolSpan());
                        break;
                    case '}':
                        yield return new CloseBraceToken(SymbolSpan());
                        break;
                    case '(':
                        yield return new OpenParenToken(SymbolSpan());
                        break;
                    case ')':
                        yield return new CloseParenToken(SymbolSpan());
                        break;
                    case '[':
                        yield return new OpenBracketToken(SymbolSpan());
                        break;
                    case ']':
                        yield return new CloseBracketToken(SymbolSpan());
                        break;
                    case ';':
                        yield return new SemicolonToken(SymbolSpan());
                        break;
                    case ',':
                        yield return new CommaToken(SymbolSpan());
                        break;
                    case '.':
                        if (NextCharIs('.'))
                        {
                            // it is `..`
                            yield return new DotDotToken(SymbolSpan(2));
                        }
                        else
                            yield return new DotToken(SymbolSpan());
                        break;
                    case ':':
                        yield return new ColonToken(SymbolSpan());
                        break;
                    case '?':
                        yield return new QuestionToken(SymbolSpan());
                        break;
                    case '|':
                        yield return new PipeToken(SymbolSpan());
                        break;
                    case '$':
                        yield return new DollarToken(SymbolSpan());
                        break;
                    case '→':
                        yield return new RightArrowToken(SymbolSpan());
                        break;
                    case '@':
                        yield return new AtSignToken(SymbolSpan());
                        break;
                    case '^':
                        yield return new CaretToken(SymbolSpan());
                        break;
                    case '+':
                        if (NextCharIs('='))
                            // it is `+=`
                            yield return new PlusEqualsToken(SymbolSpan(2));
                        else
                            // it is `+`
                            yield return new PlusToken(SymbolSpan());
                        break;
                    case '-':
                        switch (NextChar())
                        {
                            case '=':
                                // it is `-=`
                                yield return new MinusEqualsToken(SymbolSpan(2));
                                break;
                            case '>':
                                // it is `->`
                                yield return new RightArrowToken(SymbolSpan(2));
                                break;
                            default:
                                // it is `-`
                                yield return new MinusToken(SymbolSpan());
                                break;
                        }
                        break;
                    case '*':
                        if (NextCharIs('='))
                            // it is `*=`
                            yield return new AsteriskEqualsToken(SymbolSpan(2));
                        else
                            // it is `*`
                            yield return new AsteriskToken(SymbolSpan());
                        break;
                    case '/':
                        switch (NextChar())
                        {
                            case '/':
                                // it is a line comment `//`
                                tokenEnd = tokenStart + 2;
                                // Include newline at end
                                while (tokenEnd < text.Length)
                                {
                                    currentChar = text[tokenEnd];
                                    tokenEnd += 1;
                                    if (currentChar == '\r' || currentChar == '\n')
                                        break;
                                }

                                yield return new CommentToken(TokenSpan());
                                break;
                            case '*':
                                // it is a block comment `/*`
                                tokenEnd = tokenStart + 2;
                                var lastCharWasStar = false;
                                // Include slash at end
                                for (; ; )
                                {
                                    // If we ran into the end of the file, error
                                    if (tokenEnd >= text.Length)
                                    {
                                        diagnostics.Publish(LexError.UnclosedBlockComment(file,
                                            TextSpan.FromStartEnd(tokenStart, tokenEnd)));
                                        break;
                                    }
                                    currentChar = text[tokenEnd];
                                    tokenEnd += 1;
                                    if (lastCharWasStar && currentChar == '/')
                                        break;
                                    lastCharWasStar = currentChar == '*';
                                }

                                yield return new CommentToken(TokenSpan());
                                break;
                            case '=':
                                // it is `/=`
                                yield return new SlashEqualsToken(SymbolSpan(2));
                                break;
                            default:
                                // it is `/`
                                yield return new SlashToken(SymbolSpan());
                                break;
                        }
                        break;
                    case '=':
                        if (NextCharIs('='))
                            // it is `==`
                            yield return new EqualsEqualsToken(SymbolSpan(2));
                        else if (NextCharIs('/') && CharIs(2, '='))
                            // it is `=/=`
                            yield return new NotEqualToken(SymbolSpan(3));
                        else
                            // it is `=`
                            yield return new EqualsToken(SymbolSpan());
                        break;
                    case '≠':
                        yield return new NotEqualToken(SymbolSpan());
                        break;
                    case '>':
                        if (NextCharIs('='))
                            // it is `>=`
                            yield return new GreaterThanOrEqualToken(SymbolSpan(2));
                        else
                            // it is `>`
                            yield return new GreaterThanToken(SymbolSpan());
                        break;
                    case '≥':
                    case '⩾':
                        yield return new GreaterThanOrEqualToken(SymbolSpan());
                        break;
                    case '<':
                        if (NextCharIs('='))
                            // it is `<=`
                            yield return new LessThanOrEqualToken(SymbolSpan(2));
                        else
                            // it is `<`
                            yield return new LessThanToken(SymbolSpan());
                        break;
                    case '≤':
                    case '⩽':
                        yield return new LessThanOrEqualToken(SymbolSpan());
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
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < text.Length && IsIntegerCharacter(text[tokenEnd]))
                                tokenEnd += 1;

                            var span = TokenSpan();
                            var value = BigInteger.Parse(code[span]);
                            yield return new IntegerLiteralToken(span, value);
                            break;
                        }
                    case '\\':
                        {
                            tokenEnd = tokenStart + 1;
                            while (tokenEnd < text.Length && IsIdentifierCharacter(text[tokenEnd]))
                                tokenEnd += 1;

                            var identifierStart = tokenStart + 1;
                            yield return new EscapedIdentifierToken(TokenSpan(), text.Substring(identifierStart, tokenEnd - identifierStart));
                            break;
                        }
                    default:
                        if (char.IsWhiteSpace(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            // Include whitespace at end
                            while (tokenEnd < text.Length && char.IsWhiteSpace(text[tokenEnd]))
                                tokenEnd += 1;

                            yield return new WhitespaceToken(TokenSpan());
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
                            var span = SymbolSpan(2);
                            diagnostics.Publish(LexError.CStyleNotEquals(file, span));
                            yield return new NotEqualToken(span);
                        }
                        else
                        {
                            var span = SymbolSpan();
                            diagnostics.Publish(LexError.UnexpectedCharacter(file, span, currentChar));
                            yield return new UnexpectedToken(span);
                        }
                        break;
                }
                tokenStart = tokenEnd;
            }

            // The end of file token provides something to attach any final errors to
            yield return new EndOfFileToken(SymbolSpan(0), diagnostics.Build());
            yield break;

            TextSpan SymbolSpan(int length = 1)
            {
                var end = tokenStart + length;
                return TokenSpan(end);
            }

            TextSpan TokenSpan(int? end = null)
            {
                tokenEnd = end ?? tokenEnd;
                return TextSpan.FromStartEnd(tokenStart, tokenEnd);
            }
            Token NewIdentifierOrKeywordToken()
            {
                var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
                var value = code[span];
                if (Tokens.TokenTypes.KeywordFactories.TryGetValue(value, out var keywordFactory))
                    return keywordFactory(span);
                if (Tokens.TokenTypes.BooleanOperatorFactories.TryGetValue(value, out var operatorFactory))
                    return operatorFactory(span);

                return new BareIdentifierToken(span, value);
            }

            char? NextChar()
            {
                var index = tokenStart + 1;
                return index < text.Length ? text[index] : default;
            }

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
                    // In case of an invalid escape sequence, we just drop the `\` from the value

                    if (tokenEnd >= text.Length)
                    {
                        // Just the slash is invalid
                        var errorSpan = TextSpan.FromStartEnd(tokenEnd - 1, tokenEnd);
                        diagnostics.Publish(LexError.InvalidEscapeSequence(file, errorSpan));
                        break; // we hit the end of file and need to not add to tokenEnd any more
                    }

                    // Escape Sequence with next char (i.e. "\\x")
                    var escapeStart = tokenEnd - 1;
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
                            {
                                if (tokenEnd < text.Length && text[tokenEnd] == '(')
                                    tokenEnd += 1;
                                else
                                {
                                    content.Append('u');
                                    var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                                    diagnostics.Publish(LexError.InvalidEscapeSequence(file, errorSpan));
                                    break;
                                }

                                var codepoint = new StringBuilder(6);
                                while (tokenEnd < text.Length &&
                                       IsHexDigit(currentChar = text[tokenEnd]))
                                {
                                    codepoint.Append(currentChar);
                                    tokenEnd += 1;
                                }

                                int value;
                                if (codepoint.Length > 0
                                    && codepoint.Length <= 6
                                    && (value = Convert.ToInt32(codepoint.ToString(), 16)) <= 0x10FFFF)
                                {
                                    // TODO disallow surrogate pairs
                                    content.Append(char.ConvertFromUtf32(value));
                                }
                                else
                                {
                                    content.Append("u(");
                                    content.Append(codepoint);
                                    // Include the closing ')' in the escape sequence if it is present
                                    if (tokenEnd < text.Length && text[tokenEnd] == ')')
                                    {
                                        content.Append(')');
                                        tokenEnd += 1;
                                    }
                                    var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                                    diagnostics.Publish(LexError.InvalidEscapeSequence(file, errorSpan));
                                    break;
                                }

                                if (tokenEnd < text.Length && text[tokenEnd] == ')')
                                    tokenEnd += 1;
                                else
                                {
                                    var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                                    diagnostics.Publish(LexError.InvalidEscapeSequence(file, errorSpan));
                                }
                                break;
                            }
                        default:
                            {
                                // Last two chars form the invalid sequence
                                var errorSpan = TextSpan.FromStartEnd(tokenEnd - 2, tokenEnd);
                                diagnostics.Publish(LexError.InvalidEscapeSequence(file, errorSpan));
                                // drop the `/` keep the character after
                                content.Append(currentChar);
                                break;
                            }
                    }
                }

                if (tokenEnd < text.Length)
                {
                    // To include the close quote
                    if (text[tokenEnd] == '"') tokenEnd += 1;
                }
                else
                    diagnostics.Publish(LexError.UnclosedStringLiteral(file,
                        TextSpan.FromStartEnd(tokenStart, tokenEnd)));

                return new StringLiteralToken(TextSpan.FromStartEnd(tokenStart, tokenEnd), content.ToString());
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
