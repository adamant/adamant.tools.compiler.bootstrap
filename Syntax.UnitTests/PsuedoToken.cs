using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core.Tests;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests
{
    public class PsuedoToken
    {
        public readonly TokenKind Kind;
        public readonly string Text;
        public readonly object Value;

        public PsuedoToken(TokenKind kind, string text, object value = null)
        {
            Kind = kind;
            Text = text;
            Value = value;
        }

        public static PsuedoToken EndOfFile()
        {
            return new PsuedoToken(TokenKind.EndOfFile, "", new List<Diagnostic>().AsReadOnly());
        }

        public static PsuedoToken For(Token token, CodeText code)
        {
            return new PsuedoToken(token.Kind, token.Text(code), token.Value);
        }

        public CodeFile ToFakeCodeFile()
        {
            return Text.ToFakeCodeFile();
        }

        public override bool Equals(object obj)
        {
            if (obj is PsuedoToken token &&
                Kind == token.Kind &&
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
            return HashCode.Combine(Kind, Text, Value);
        }

        public override string ToString()
        {
            var textValue = string.IsNullOrEmpty(Text) ? "" : $":„{Text}„";
            switch (Value)
            {
                case null:
                    return $"{Kind}{textValue}";
                case string s:
                    return $"{Kind}{textValue} 【{Regex.Escape(s)}】";
                case BigInteger i:
                    return $"{Kind}{textValue} {i}";
                case IReadOnlyList<Diagnostic> diagnostics:
                    return $"{Kind}{textValue} [{diagnostics.DebugFormat()}]";
                default:
                    return $"{Kind}{textValue} InvalidValue={Value}";
            }
        }
    }
}
