using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    /// <summary>
    /// This is the type of integer constants, it isn't possible to declare a
    /// variable to have this type. It will never be inferred as the type of a
    /// variable. It is not a `KnownType` because all expressions should have
    /// their types inferred to some specific type.
    /// </summary>
    public class IntegerConstantType : UnresolvedType
    {
        public readonly BigInteger Value;

        public IntegerConstantType(BigInteger value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString().AssertNotNull();
        }
    }
}
