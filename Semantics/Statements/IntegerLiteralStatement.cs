using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class IntegerLiteralStatement : SimpleStatement
    {

        [NotNull] public LValue LValue { get; }
        public BigInteger Value { get; }


        public IntegerLiteralStatement(
            [NotNull] LValue lValue,
             BigInteger value)
        {
            Requires.NotNull(nameof(lValue), lValue);
            LValue = lValue;
            Value = value;
        }
    }
}
