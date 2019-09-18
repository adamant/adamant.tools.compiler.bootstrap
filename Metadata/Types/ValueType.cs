using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(PointerType),
        typeof(SimpleType),
        typeof(OptionalType))]
    public abstract class ValueType : DataType
    {
    }
}
