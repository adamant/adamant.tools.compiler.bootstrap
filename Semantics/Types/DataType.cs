namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public abstract class DataType
    {
        public static readonly UnknownType Unknown = UnknownType.Instance;

        public abstract override string ToString();
    }
}
