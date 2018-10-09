using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class StringLiteralToken : Token
    {
        public readonly string Value;

        public StringLiteralToken(TextSpan span, string value)
            : base(span)
        {
            Value = value;
        }
    }
}
