using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        //typeof(FloatingPointType),
        typeof(IntegerType))]
    public abstract class NumericType : SimpleType
    {
        private protected NumericType(SpecialTypeName name)
            : base(name)
        {
        }
    }
}
