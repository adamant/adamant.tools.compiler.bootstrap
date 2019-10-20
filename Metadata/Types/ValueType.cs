using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(SimpleType),
        typeof(OptionalType))]
    public abstract class ValueType : DataType
    {
    }
}
