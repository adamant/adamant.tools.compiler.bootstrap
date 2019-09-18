using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        //typeof(FloatingPointType),
        typeof(IntegerType))]
    public abstract class NumericType : SimpleType
    {
        protected NumericType(string name)
            : base(name)
        {
        }
    }
}
