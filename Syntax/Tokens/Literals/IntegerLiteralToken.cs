using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Literals
{
    public class IntegerLiteralToken : Token
    {
        public readonly BigInteger Value;

        public IntegerLiteralToken(TextSpan span, BigInteger value)
            : base(span)
        {
            Value = value;
        }
    }
}
