using System;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// While namespaces in syntax declarations are repeated across files, and
    /// IL doesn't even directly represent namespaces, for symbols, a namespace
    /// is the container of all the names in it. There is one symbol per namespace.
    /// </summary>
    public sealed class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(Name fullName)
            : base(fullName) { }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is NamespaceSymbol otherNamespace
                   && FullName == otherNamespace.FullName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName);
        }
    }
}
