using System.Globalization;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class IntegerLiteralExpressionSyntax : LiteralExpressionSyntax, IIntegerLiteralExpressionSyntax
    {
        public BigInteger Value { get; }

        public IntegerLiteralExpressionSyntax(TextSpan span, BigInteger value)
            : base(span, ExpressionSemantics.Copy)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
