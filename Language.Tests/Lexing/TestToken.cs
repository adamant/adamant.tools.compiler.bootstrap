using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class TestToken : IXunitSerializable
    {
        public TestTokenKind Kind { get; private set; }
        public string Text { get; private set; }
        public bool IsValid { get; private set; }
        public object Value { get; private set; }

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

        public static TestToken Invalid(TokenKind kind, string text, object value)
        {
            return new TestToken(TestTokenKind.Token(kind), text, false, value);
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

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Kind = info.GetValue<TestTokenKind>("Kind");
            Text = info.GetValue<string>("Text");
            IsValid = info.GetValue<bool>("IsValid");
            Value = info.GetValue<object>("Value");
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Kind", Kind);
            info.AddValue("Text", Text);
            info.AddValue("IsValid", IsValid);
            info.AddValue("Value", Value);
        }
    }
}
