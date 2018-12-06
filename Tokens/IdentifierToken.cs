using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal abstract class IdentifierToken : Token, IIdentifierToken
    {
        public string Value { get; }

        protected IdentifierToken(TextSpan span, string value)
            : base(span)
        {
            Value = value;
        }

        // Helpful for debugging
        public override string ToString()
        {
            return Value;
        }
    }
}
