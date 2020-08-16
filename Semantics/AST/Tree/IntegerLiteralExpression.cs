using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class IntegerLiteralExpression : LiteralExpression, IIntegerLiteralExpression
    {
        public BigInteger Value { get; }

        public IntegerLiteralExpression(TextSpan span, DataType dataType, BigInteger value)
            : base(span, dataType)
        {
            Value = value;
        }
    }
}
