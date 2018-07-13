using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class TestTokenSequence : IXunitSerializable
    {
        private TestToken[] tokens;

        public IEnumerable<TestToken> Tokens => tokens;

        public TestTokenSequence()
        {
            tokens = new TestToken[0];
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
            if (tokens.All(IsToken))
                return this;
            else
                return new TestTokenSequence(tokens.Where(IsToken).ToArray());
        }

        private static bool IsToken(TestToken t)
        {
            return t.Kind.Category == TestTokenCategory.Token;
        }

        public override string ToString()
        {
            return string.Concat(tokens.Select(t => t.Text));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            tokens = info.GetValue<TestToken[]>("tokens");
        }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("tokens", tokens);
        }
    }
}
