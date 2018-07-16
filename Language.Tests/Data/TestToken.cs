using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class TestToken : IXunitSerializable
    {
        public bool Permute { get; private set; }
        public TestTokenKind Kind { get; private set; }
        public string Text { get; private set; }
        public bool IsValid { get; private set; }
        public object Value { get; private set; }

        [Obsolete("Required by IXunitSerializable", true)]
        public TestToken()
        {
        }

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

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Permute), Permute);
            info.AddValue(nameof(Kind), Kind);
            info.AddValue(nameof(Text), Text);
            info.AddValue(nameof(IsValid), IsValid);
            info.AddValue(nameof(Value), Value);
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Permute = info.GetValue<bool>(nameof(Permute));
            Kind = info.GetValue<TestTokenKind>(nameof(Kind));
            Text = info.GetValue<string>(nameof(Text));
            IsValid = info.GetValue<bool>(nameof(IsValid));
            Value = info.GetValue<object>(nameof(Value));
        }
    }
}
