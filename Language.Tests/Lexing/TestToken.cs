using Adamant.Tools.Compiler.Bootstrap.Core;
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
            Requires.NotNull(nameof(text), text);
            Permute = permute;
            Kind = kind;
            Text = text;
            IsValid = isValid;
            Value = value;
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
