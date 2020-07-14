using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        //typeof(FloatingPointType),
        typeof(IntegerType))]
    public abstract class NumericType : SimpleType
    {
        private protected NumericType(string name)
            : base(name)
        {
        }
    }
}
