namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// An unresolved type is a type that either unknown or an overloaded type.
    /// Hopefully, it will be resolved to a specific type.
    /// </summary>
    public abstract class UnresolvedType : DataType
    {
        public override bool IsResolved => false;
    }
}
