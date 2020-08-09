using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        typeof(IntegerConstantType),
        typeof(FixedSizeIntegerType),
        typeof(PointerSizedIntegerType))]
    public abstract class IntegerType : NumericType
    {
        private protected IntegerType(SpecialTypeName name)
            : base(name)
        {
        }
    }
}
