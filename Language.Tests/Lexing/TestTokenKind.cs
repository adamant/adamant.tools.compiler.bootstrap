using System;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public struct TestTokenKind
    {
        public readonly TestTokenCategory Category;
        public readonly TokenKind? TokenKind;

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

        public TokenKind ToTokenKind()
        {
            if (Category != TestTokenCategory.Token)
                throw new NotSupportedException($"Can't convert TestTokenKind with Category=`{Category}` to TokenKind");
            return TokenKind.Value;
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
            if (Category == TestTokenCategory.Token)
                return TokenKind.ToString();
            return Category.ToString();
        }
    }
}
