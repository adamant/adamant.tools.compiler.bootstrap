using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    public class PsuedoToken
    {
        public Type TokenType { get; }

        public string Text { get; }
        public object? Value { get; }

        public PsuedoToken(Type tokenType, string text, object? value = null)
        {
            TokenType = tokenType;
            Text = text;
            Value = value;
        }

        public static PsuedoToken EndOfFile()
        {
            return new PsuedoToken(typeof(IEndOfFileToken), "");
        }

        public static PsuedoToken For(IToken token, CodeText code)
        {
            var tokenType = token.GetType();
            var text = token.Text(code);
            return token switch
            {
                IIdentifierToken identifier => new PsuedoToken(tokenType, text, identifier.Value),
                IStringLiteralToken stringLiteral => new PsuedoToken(tokenType, text, stringLiteral.Value),
                IIntegerLiteralToken integerLiteral => new PsuedoToken(tokenType, text, integerLiteral.Value),
                _ => new PsuedoToken(tokenType, text)
            };
        }

        public override bool Equals(object? obj)
        {
            if (obj is PsuedoToken token &&
                (TokenType == token.TokenType
                    || TokenType.IsAssignableFrom(token.TokenType)
                    || token.TokenType.IsAssignableFrom(TokenType)) &&
                Text == token.Text)
            {
                if (Value is IReadOnlyList<Diagnostic> diagnostics
                    && token.Value is IReadOnlyList<Diagnostic> otherDiagnostics)
                {
                    // TODO this zip looks wrong, shouldn't it be comparing something rather than always returning false?
                    return diagnostics.Zip(otherDiagnostics, (d1, d2) => false).All(i => i);
                }
                return EqualityComparer<object>.Default.Equals(Value, token.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TokenType, Text, Value);
        }

        public override string ToString()
        {
            var textValue = string.IsNullOrEmpty(Text) ? "" : $":„{Text.Escape()}„";
            return Value switch
            {
                null => $"{TokenType.Name}{textValue}",
                string s => $"{TokenType.Name}{textValue} 【{s.Escape()}】",
                BigInteger i => $"{TokenType.Name}{textValue} {i}",
                IReadOnlyList<Diagnostic> diagnostics => $"{TokenType.Name}{textValue} [{diagnostics.DebugFormat()}]",
                _ => $"{TokenType.Name}{textValue} InvalidValue={Value}"
            };
        }
    }
}
