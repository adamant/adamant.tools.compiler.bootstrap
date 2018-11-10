using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers
{
    public class PsuedoToken
    {
        [NotNull]
        public readonly Type TokenType;

        [NotNull]
        public readonly string Text;
        public readonly object Value;

        public PsuedoToken([NotNull] Type tokenType, [NotNull] string text, object value = null)
        {
            TokenType = tokenType;
            Text = text;
            Value = value;
        }

        public static PsuedoToken EndOfFile()
        {
            return new PsuedoToken(typeof(IEndOfFileToken), "", new List<Diagnostic>().AsReadOnly());
        }

        public static PsuedoToken For([NotNull] ITokenPlace token, [NotNull] CodeText code)
        {
            var tokenType = token.GetType().NotNull();
            switch (token)
            {
                case IIdentifierTokenPlace identifier:
                    return new PsuedoToken(tokenType, token.Text(code), identifier.Value);
                case IStringLiteralToken stringLiteral:
                    return new PsuedoToken(tokenType, token.Text(code), stringLiteral.Value);
                case IIntegerLiteralToken integerLiteral:
                    return new PsuedoToken(tokenType, token.Text(code), integerLiteral.Value);
                case IEndOfFileToken eof:
                    return new PsuedoToken(tokenType, token.Text(code), eof.Diagnostics);
                case ITokenPlace _:
                    return new PsuedoToken(tokenType, token.Text(code));
                default:
                    throw NonExhaustiveMatchException.For(token);
            }
        }

        [NotNull]
        public CodeFile ToFakeCodeFile()
        {
            return FakeCodeFile.For(Text);
        }

        public override bool Equals(object obj)
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
            switch (Value)
            {
                case null:
                    return $"{TokenType.Name}{textValue}";
                case string s:
                    return $"{TokenType.Name}{textValue} 【{s.Escape()}】";
                case BigInteger i:
                    return $"{TokenType.Name}{textValue} {i}";
                case IReadOnlyList<Diagnostic> diagnostics:
                    return $"{TokenType.Name}{textValue} [{diagnostics.DebugFormat()}]";
                default:
                    return $"{TokenType.Name}{textValue} InvalidValue={Value}";
            }
        }
    }
}
