using System;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// An unresolved type is a type that either unknown or an overloaded type.
    /// Hopefully, it will be resolved to a specific type.
    /// </summary>
    [Obsolete("Unknown will be the only unresolved type once overloaded types are removed")]
    public abstract class UnresolvedType : DataType
    {
        public override bool IsResolved => false;
    }
}
