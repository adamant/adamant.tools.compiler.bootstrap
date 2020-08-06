using System;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// While namespaces in syntax declarations are repeated across files, and
    /// IL doesn't even directly represent namespaces, for symbols, a namespace
    /// is the container of all the names in it. There is one symbol per namespace.
    /// </summary>
    public sealed class NamespaceSymbol : NamespaceOrPackageSymbol
    {
        public new NamespaceOrPackageSymbol ContainingSymbol { get; }

        public NamespaceSymbol(NamespaceOrPackageSymbol containingSymbol, Name name)
            : base(containingSymbol, name)
        {
            ContainingSymbol = containingSymbol;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is NamespaceSymbol otherNamespace
                   && ContainingSymbol.Equals(otherNamespace.ContainingSymbol)
                   && Name == otherNamespace.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainingSymbol, Name);
        }
    }
}
