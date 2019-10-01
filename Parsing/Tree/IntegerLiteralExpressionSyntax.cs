using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class IntegerLiteralExpressionSyntax : LiteralExpressionSyntax, IIntegerLiteralExpressionSyntax
    {
        public BigInteger Value { get; }

        public IntegerLiteralExpressionSyntax(TextSpan span, BigInteger value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
