using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Core.Syntax;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public struct TestTokenKind : IXunitSerializable
    {
        public TestTokenCategory Category { get; private set; }
        public TokenKind? TokenKind { get; private set; }

        private TestTokenKind(TestTokenCategory category, TokenKind? tokenKind)
        {
            Category = category;
            TokenKind = tokenKind;
        }

        public static TestTokenKind Whitespace()
        {
            return new TestTokenKind(TestTokenCategory.Whitespace, null);
        }

        public static TestTokenKind Comment()
        {
            return new TestTokenKind(TestTokenCategory.Comment, null);
        }

        public static TestTokenKind Token(TokenKind kind)
        {
            return new TestTokenKind(TestTokenCategory.Token, kind);
        }

        public static TestTokenKind AnyToken()
        {
            return new TestTokenKind(TestTokenCategory.Token, null);
        }

        public static TestTokenKind Keyword()
        {
            return new TestTokenKind(TestTokenCategory.Keyword, null);
        }

        public static TestTokenKind Keyword(TokenKind kind)
        {
            return new TestTokenKind(TestTokenCategory.Keyword, kind);
        }

        public TokenKind ToTokenKind()
        {
            if (!(Category == TestTokenCategory.Token
                || (Category == TestTokenCategory.Keyword && TokenKind != null)))
                throw new NotSupportedException($"Can't convert TestTokenKind with Category=`{Category}` to TokenKind");
            return TokenKind.Value;
        }

        public bool Matches(TestTokenKind other)
        {
            return Category == other.Category
                && (TokenKind == null || TokenKind == other.TokenKind);
        }

        public static bool operator ==(TestTokenKind lhs, TestTokenKind rhs)
        {
            return lhs.Category == rhs.Category && lhs.TokenKind == rhs.TokenKind;
        }
        public static bool operator !=(TestTokenKind lhs, TestTokenKind rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj)
        {
            var rhs = obj as TestTokenKind?;
            return this == rhs;
        }
        public override int GetHashCode()
        {
            return Category.GetHashCode() ^ TokenKind.GetHashCode();
        }
        public override string ToString()
        {
            if (TokenKind != null)
                return TokenKind.ToString();
            return Category.ToString();
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Category), Category);
            info.AddValue(nameof(TokenKind), TokenKind);
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Category = info.GetValue<TestTokenCategory>(nameof(Category));
            TokenKind = info.GetValue<TokenKind?>(nameof(TokenKind));
        }
    }
}
