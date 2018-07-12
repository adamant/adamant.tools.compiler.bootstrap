using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public struct TestTokenMatcher
    {
        public readonly TestTokenKind TestKind;
        public readonly string Text;

        public TestTokenMatcher(TestTokenKind testKind, string text)
        {
            TestKind = testKind;
            Text = text;
        }

        public bool Matches(TestToken token)
        {
            return TestKind == token.Kind
                && (Text == null || Text == token.Text);
        }
    }
}
