using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        typeof(SimpleType),
        typeof(OptionalType))]
    public abstract class ValueType : DataType
    {
        private protected ValueType() { }
    }
}
