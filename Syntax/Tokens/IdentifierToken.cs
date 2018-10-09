using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class IdentifierToken : Token
    {
        public string Value { get; }

        public IdentifierToken(TextSpan span, string value)
            : base(span)
        {
            Value = value;
        }
    }
}
