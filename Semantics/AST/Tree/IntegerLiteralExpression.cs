using System.Globalization;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class IntegerLiteralExpression : LiteralExpression, IIntegerLiteralExpression
    {
        public BigInteger Value { get; }

        public IntegerLiteralExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            BigInteger value)
            : base(span, dataType, semantics)
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
