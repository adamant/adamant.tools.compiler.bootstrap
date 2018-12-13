namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class ValueType : DataType
    {
        public abstract ValueSemantics Semantics { get; }
    }
}
