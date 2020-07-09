using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(IntegerConstantType),
        typeof(SizedIntegerType),
        typeof(UnsizedIntegerType))]
    public abstract class IntegerType : NumericType
    {
        private protected IntegerType(string name)
            : base(name)
        {
        }
    }
}
