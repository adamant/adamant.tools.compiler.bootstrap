using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class TestTokenSequence : IXunitSerializable
    {
        private TestToken[] tokens;

        public IReadOnlyList<TestToken> Tokens => tokens;

        public TestTokenSequence()
        {
            tokens = new TestToken[0];
        }

        public TestTokenSequence(IEnumerable<TestToken> tokens)
        {
            this.tokens = tokens.ToArray();
        }

        private TestTokenSequence(TestToken[] tokens)
        {
            this.tokens = tokens;
        }

        public static TestTokenSequence Single(TestToken current)
        {
            Requires.NotNull(nameof(current), current);
            return new TestTokenSequence(new[] { current });
        }

        public TestTokenSequence Append(TestToken current)
        {
            Requires.NotNull(nameof(current), current);
            var newTokens = new TestToken[tokens.Length + 1];
            tokens.CopyTo(newTokens, 0);
            newTokens[tokens.Length] = current;
            return new TestTokenSequence(newTokens);
        }

        public TestTokenSequence WhereIsToken()
        {
            return tokens.All(IsToken) ? this : new TestTokenSequence(tokens.Where(IsToken).ToArray());
        }

        private static bool IsToken(TestToken t)
        {
            return t.Kind.Category == TestTokenCategory.Token
                || t.Kind.Category == TestTokenCategory.Keyword;
        }

        public override string ToString()
        {
            return string.Concat(tokens.Select(t => t.Text));
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(tokens), tokens);
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            tokens = info.GetValue<TestToken[]>(nameof(tokens));
        }
    }
}
