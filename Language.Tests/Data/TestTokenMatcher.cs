namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public struct TestTokenMatcher
    {
        public readonly TestTokenKind Kind;
        public readonly string Text;

        public TestTokenMatcher(TestTokenKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public bool Matches(TestToken token)
        {
            return Kind.Matches(token.Kind)
                && (Text == null || Text == token.Text);
        }
    }
}
