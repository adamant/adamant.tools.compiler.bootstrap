using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public class Lexer
    {
        [MustUseReturnValue]
        [NotNull]
        public ITokenIterator Lex([NotNull] ParseContext context)
        {
            return new TokenIterator(context, Lex(context.File, context.Diagnostics));
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        private static IEnumerable<IToken> Lex([NotNull] CodeFile file, [NotNull] Diagnostics diagnostics)
        {
            var code = file.Code;
            var text = code.Text;
            var tokenStart = 0;
            var tokenEnd = -1; // One past the end position to allow for zero length spans
            while (tokenStart < text.Length)
            {
                var currentChar = text[tokenStart];
                switch (currentChar)
                {
                    case '{':
                        yield return TokenFactory.OpenBrace(SymbolSpan());
                        break;
                    case '}':
                        yield return TokenFactory.CloseBrace(SymbolSpan());
                        break;
                    case '(':
                        yield return TokenFactory.OpenParen(SymbolSpan());
                        break;
                    case ')':
                        yield return TokenFactory.CloseParen(SymbolSpan());
                        break;
                    case '[':
                        yield return TokenFactory.OpenBracket(SymbolSpan());
                        break;
                    case ']':
                        yield return TokenFactory.CloseBracket(SymbolSpan());
                        break;
                    case ';':
                        yield return TokenFactory.Semicolon(SymbolSpan());
                        break;
                    case ',':
                        yield return TokenFactory.Comma(SymbolSpan());
                        break;
                    case '#':
                        if (NextCharIs('#'))
                            // it is `##`
                            yield return TokenFactory.HashHash(SymbolSpan(2));
                        else
                            // it is `#`
                            yield return TokenFactory.Hash(SymbolSpan(1));
                        break;
                    case '.':
                        if (NextCharIs('.'))
                        {
                            if (CharAtIs(2, '<'))
                                // it is `..<`
                                yield return TokenFactory.DotDotLessThan(SymbolSpan(3));
                            else
                                // it is `..`
                                yield return TokenFactory.DotDot(SymbolSpan(2));
                        }
                        else
                            yield return TokenFactory.Dot(SymbolSpan());
                        break;
                    case ':':
                        if (NextCharIs(':'))
                            // it is `::`
                            yield return TokenFactory.ColonColon(SymbolSpan(2));
                        else
                            // it is `:`
                            yield return TokenFactory.Colon(SymbolSpan());
                        break;
                    case '?':
                        switch (NextChar())
                        {
                            case '?':
                                // it is `??`
                                yield return TokenFactory.QuestionQuestion(SymbolSpan(2));
                                break;
                            case '.':
                                // it is `?.`
                                yield return TokenFactory.QuestionDot(SymbolSpan(2));
                                break;
                            default:
                                // it is `?`
                                yield return TokenFactory.Question(SymbolSpan());
                                break;
                        }
                        break;
                    case '|':
                        yield return TokenFactory.Pipe(SymbolSpan());
                        break;
                    case '$':
                        switch (NextChar())
                        {
                            case '<':
                                switch (CharAt(2))
                                {
                                    case '≠':
                                        // it is `$<≠`
                                        yield return TokenFactory.DollarLessThanNotEqual(SymbolSpan(3));
                                        break;
                                    case '/':
                                        if (CharAtIs(3, '='))
                                            // it is `$</=`
                                            yield return TokenFactory.DollarLessThanNotEqual(
                                                SymbolSpan(4));
                                        else
                                            goto default;
                                        break;
                                    default:
                                        // it is `$<`
                                        yield return TokenFactory.DollarLessThan(SymbolSpan(2));
                                        break;
                                }
                                break;
                            case '>':
                                switch (CharAt(2))
                                {
                                    case '≠':
                                        // it is `$>≠`
                                        yield return TokenFactory.DollarGreaterThanNotEqual(SymbolSpan(3));
                                        break;
                                    case '/':
                                        if (CharAtIs(3, '='))
                                            // it is `$>/=`
                                            yield return TokenFactory.DollarGreaterThanNotEqual(
                                                SymbolSpan(4));
                                        else
                                            goto default;
                                        break;
                                    default:
                                        // it is `$>`
                                        yield return TokenFactory.DollarGreaterThan(SymbolSpan(2));
                                        break;
                                }
                                break;
                            default:
                                // it is `=`
                                yield return TokenFactory.Dollar(SymbolSpan());
                                break;
                        }
                        break;
                    case '→':
                        yield return TokenFactory.RightArrow(SymbolSpan());
                        break;
                    case '@':
                        yield return TokenFactory.AtSign(SymbolSpan());
                        break;
                    case '^':
                        if (NextCharIs('.'))
                            // it is `^.`
                            yield return TokenFactory.CaretDot(SymbolSpan(2));
                        else
                            // it is `^`
                            yield return TokenFactory.Caret(SymbolSpan());
                        break;
                    case '+':
                        if (NextCharIs('='))
                            // it is `+=`
                            yield return TokenFactory.PlusEquals(SymbolSpan(2));
                        else
                            // it is `+`
                            yield return TokenFactory.Plus(SymbolSpan());
                        break;
                    case '-':
                        switch (NextChar())
                        {
                            case '=':
                                // it is `-=`
                                yield return TokenFactory.MinusEquals(SymbolSpan(2));
                                break;
                            case '>':
                                // it is `->`
                                yield return TokenFactory.RightArrow(SymbolSpan(2));
                                break;
                            default:
                                // it is `-`
                                yield return TokenFactory.Minus(SymbolSpan());
                                break;
                        }
                        break;
                    case '*':
                        if (NextCharIs('='))
                            // it is `*=`
                            yield return TokenFactory.AsteriskEquals(SymbolSpan(2));
                        else
                            // it is `*`
                            yield return TokenFactory.Asterisk(SymbolSpan());
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

                                yield return TokenFactory.Comment(TokenSpan());
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
                                        diagnostics.Add(LexError.UnclosedBlockComment(file,
                                            TextSpan.FromStartEnd(tokenStart, tokenEnd)));
                                        break;
                                    }
                                    currentChar = text[tokenEnd];
                                    tokenEnd += 1;
                                    if (lastCharWasStar && currentChar == '/')
                                        break;
                                    lastCharWasStar = currentChar == '*';
                                }

                                yield return TokenFactory.Comment(TokenSpan());
                                break;
                            case '=':
                                // it is `/=`
                                yield return TokenFactory.SlashEquals(SymbolSpan(2));
                                break;
                            default:
                                // it is `/`
                                yield return TokenFactory.Slash(SymbolSpan());
                                break;
                        }
                        break;
                    case '=':
                        switch (NextChar())
                        {
                            case '>':
                                // it is `=>`
                                yield return TokenFactory.EqualsGreaterThan(SymbolSpan(2));
                                break;
                            case '=':
                                // it is `==`
                                yield return TokenFactory.EqualsEquals(SymbolSpan(2));
                                break;
                            case '/':
                                if (CharAtIs(2, '='))
                                    // it is `=/=`
                                    yield return TokenFactory.NotEqual(SymbolSpan(3));
                                else goto default;
                                break;
                            default:
                                // it is `=`
                                yield return TokenFactory.Equals(SymbolSpan());
                                break;
                        }
                        break;
                    case '≠':
                        yield return TokenFactory.NotEqual(SymbolSpan());
                        break;
                    case '>':
                        if (NextCharIs('='))
                            // it is `>=`
                            yield return TokenFactory.GreaterThanOrEqual(SymbolSpan(2));
                        else
                            // it is `>`
                            yield return TokenFactory.GreaterThan(SymbolSpan());
                        break;
                    case '≥':
                    case '⩾':
                        yield return TokenFactory.GreaterThanOrEqual(SymbolSpan());
                        break;
                    case '<':
                        switch (NextChar())
                        {
                            case '=':
                                // it is `<=`
                                yield return TokenFactory.LessThanOrEqual(SymbolSpan(2));
                                break;
                            case ':':
                                // it is `<:`
                                yield return TokenFactory.LessThanColon(SymbolSpan(2));
                                break;
                            case '.':
                                if (CharAtIs(2, '.'))
                                {
                                    if (CharAtIs(3, '<'))
                                        // it is `<..<`
                                        yield return TokenFactory.LessThanDotDotLessThan(SymbolSpan(4));
                                    else
                                        // it is `<..`
                                        yield return TokenFactory.LessThanDotDot(SymbolSpan(3));
                                }
                                else
                                    goto default;
                                break;
                            default:
                                // it is `<`
                                yield return TokenFactory.LessThan(SymbolSpan());
                                break;
                        }
                        break;
                    case '≤':
                    case '⩽':
                        yield return TokenFactory.LessThanOrEqual(SymbolSpan());
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
                        yield return TokenFactory.IntegerLiteral(span, value);
                        break;
                    }
                    case '\\':
                    {
                        tokenEnd = tokenStart + 1;
                        while (tokenEnd < text.Length && IsIdentifierCharacter(text[tokenEnd]))
                            tokenEnd += 1;

                        var identifierStart = tokenStart + 1;
                        yield return TokenFactory.EscapedIdentifier(TokenSpan(), text.Substring(identifierStart, tokenEnd - identifierStart));
                        break;
                    }
                    default:
                        if (char.IsWhiteSpace(currentChar))
                        {
                            tokenEnd = tokenStart + 1;
                            // Include whitespace at end
                            while (tokenEnd < text.Length && char.IsWhiteSpace(text[tokenEnd]))
                                tokenEnd += 1;

                            yield return TokenFactory.Whitespace(TokenSpan());
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
                            diagnostics.Add(LexError.CStyleNotEquals(file, span));
                            yield return TokenFactory.NotEqual(span);
                        }
                        else
                        {
                            var span = SymbolSpan();
                            diagnostics.Add(LexError.UnexpectedCharacter(file, span, currentChar));
                            yield return TokenFactory.Unexpected(span);
                        }
                        break;
                }
                tokenStart = tokenEnd;
            }

            // The end of file token provides something to attach any final errors to
            yield return TokenFactory.EndOfFile(SymbolSpan(0));
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
            IToken NewIdentifierOrKeywordToken()
            {
                var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
                var value = code[span];
                if (TokenTypes.KeywordFactories.TryGetValue(value, out var keywordFactory))
                    return keywordFactory(span);

                return TokenFactory.BareIdentifier(span, value);
            }

            char? NextChar()
            {
                var index = tokenStart + 1;
                return index < text.Length ? text[index] : default;
            }

            char? CharAt(int offset)
            {
                var index = tokenStart + offset;
                return index < text.Length ? text[index] : default;
            }

            bool NextCharIs(char c)
            {
                var index = tokenStart + 1;
                return index < text.Length && text[index] == c;
            }

            bool CharAtIs(int i, char c)
            {
                var index = tokenStart + i;
                return index < text.Length && text[index] == c;
            }

            IToken LexString()
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
                        diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
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
                                diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
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
                                diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                                break;
                            }

                            if (tokenEnd < text.Length && text[tokenEnd] == ')')
                                tokenEnd += 1;
                            else
                            {
                                var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                                diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                            }
                            break;
                        }
                        default:
                        {
                            // Last two chars form the invalid sequence
                            var errorSpan = TextSpan.FromStartEnd(tokenEnd - 2, tokenEnd);
                            diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
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
                    diagnostics.Add(LexError.UnclosedStringLiteral(file,
                        TextSpan.FromStartEnd(tokenStart, tokenEnd)));

                return TokenFactory.StringLiteral(TextSpan.FromStartEnd(tokenStart, tokenEnd), content.ToString());
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
