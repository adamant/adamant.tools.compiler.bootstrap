using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public abstract class IdentifierToken : Token
    {
        public string Value { get; }

        protected IdentifierToken(TextSpan span, string value)
            : base(span)
        {
            Value = value;
        }
    }
}
