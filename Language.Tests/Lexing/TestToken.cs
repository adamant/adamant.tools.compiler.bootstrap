using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class TestToken : IXunitSerializable
    {
        public bool Permute { get; private set; }
        public TestTokenKind Kind { get; private set; }
        public string Text { get; private set; }
        public bool IsValid { get; private set; }
        public object Value { get; private set; }

        public TestToken(bool permute, TestTokenKind kind, string text, bool isValid, object value)
        {
            Permute = permute;
            Kind = kind;
            Text = text;
            IsValid = isValid;
            Value = value;
        }

        public static TestToken Valid(bool permute, TokenKind kind, string text)
        {
            return new TestToken(permute, TestTokenKind.Token(kind), text, true, null);
        }

        public static TestToken Valid(bool permute, TokenKind kind, string text, object value)
        {
            return new TestToken(permute, TestTokenKind.Token(kind), text, true, value);
        }

        public static TestToken Invalid(bool permute, TokenKind kind, string text)
        {
            return new TestToken(permute, TestTokenKind.Token(kind), text, false, null);
        }

        public static TestToken Invalid(bool permute, TokenKind kind, string text, object value)
        {
            return new TestToken(permute, TestTokenKind.Token(kind), text, false, value);
        }

        public static TestToken Whitespace(bool permute, string text)
        {
            return new TestToken(permute, TestTokenKind.Whitespace(), text, true, null);
        }

        public static TestToken Comment(bool permute, string text)
        {
            return new TestToken(permute, TestTokenKind.Comment(), text, true, null);
        }

        public override string ToString()
        {
            var validMarker = IsValid ? "" : "*";
            return $"{Kind}{validMarker}=`{Text}`";
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Permute = info.GetValue<bool>("Permute");
            Kind = info.GetValue<TestTokenKind>("Kind");
            Text = info.GetValue<string>("Text");
            IsValid = info.GetValue<bool>("IsValid");
            Value = info.GetValue<object>("Value");
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Permute", Permute);
            info.AddValue("Kind", Kind);
            info.AddValue("Text", Text);
            info.AddValue("IsValid", IsValid);
            info.AddValue("Value", Value);
        }
    }
}
