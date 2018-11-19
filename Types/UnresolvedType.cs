namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// An unresolved type is a type that represents an unresolved type. That is,
    /// it is either unknown or a placeholder type. Hopefully, it will be resolved
    /// to a specific type.
    /// </summary>
    public abstract class UnresolvedType : DataType
    {
        public override bool IsResolved => false;
    }
}
