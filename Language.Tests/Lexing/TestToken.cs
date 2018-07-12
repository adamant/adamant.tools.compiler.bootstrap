using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class TestToken
    {
        public readonly TestTokenKind Kind;
        public readonly string Text;
        public readonly bool IsValid;
        public readonly object Value;

        public TestToken(TestTokenKind kind, string text, bool isValid, object value)
        {
            Kind = kind;
            Text = text;
            IsValid = isValid;
            Value = value;
        }

        public static TestToken Valid(TokenKind kind, string text)
        {
            return new TestToken(TestTokenKind.Token(kind), text, true, null);
        }

        public static TestToken Valid(TokenKind kind, string text, object value)
        {
            return new TestToken(TestTokenKind.Token(kind), text, true, value);
        }

        public static TestToken Invalid(TokenKind kind, string text)
        {
            return new TestToken(TestTokenKind.Token(kind), text, false, null);
        }

        public static TestToken Whitespace(string text)
        {
            return new TestToken(TestTokenKind.Whitespace(), text, true, null);
        }

        public static TestToken Comment(string text)
        {
            return new TestToken(TestTokenKind.Comment(), text, true, null);
        }

        public override string ToString()
        {
            var validMarker = IsValid ? "" : "*";
            return $"{Kind}{validMarker}=`{Text}`";
        }

        public static object[] GetSequenceData(TestToken[] sequence)
        {
            return new object[]
                {
                    string.Concat(sequence.Select(s => s.Text)),
                    sequence.Where(s => s.Kind.Category == TestTokenCategory.Token).ToArray()
                };
        }
    }
}
